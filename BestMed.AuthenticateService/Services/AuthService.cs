using System.Security.Claims;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

internal sealed class AuthService(
    IExternalAuthProvider externalAuth,
    IUserBusiness userBusiness,
    LoginResponseBuilder responseBuilder,
    IConfiguration configuration,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;
    // ── Login (Identity Server proxy) ─────────────────────────────────────────

    public async Task<IResult> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        logger.LogInformation("Login attempt for user '{Username}'", request.Username);

        try
        {
            var result = await externalAuth.LoginAsync(request, ct);
            logger.LogInformation("Login successful for user '{Username}'", request.Username);
            return Results.Ok(result);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Login failed for '{Username}' — authentication rejected", request.Username);
            return Results.Problem(
                detail: "Authentication failed. Please check your credentials.",
                statusCode: StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for '{Username}'", request.Username);
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ── Logout ────────────────────────────────────────────────────────────────

    public async Task<IResult> LogoutAsync(
        string token,
        ClaimsPrincipal user,
        string ipAddress,
        string userAgent,
        string? signOutBaseUrl,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Results.BadRequest("No access token provided.");

        var loginId = AuthHelpers.GetClaim(user, ClaimConstants.UserName);
        var originalIdStr = AuthHelpers.GetClaim(user, ClaimConstants.OriginalId);
        var userIdStr = AuthHelpers.GetClaim(user, ClaimConstants.UserId);
        var isAgency = UserRoleCodes.AgencyAndWitnessRoles.Contains(AuthHelpers.GetClaim(user, ClaimConstants.UserRole));

        if (!string.IsNullOrEmpty(loginId))
        {
            Guid? realId = Guid.TryParse(originalIdStr, out var rid) ? rid : null;
            await userBusiness.LogoutAsync(loginId, realId, ipAddress, userAgent, "LogOut", ct);

            if (isAgency && Guid.TryParse(userIdStr, out var uid))
                await userBusiness.LogoutAsync(loginId, uid, ipAddress, userAgent, "Agency LogOut", ct);
        }

        await externalAuth.LogoutAsync(token, ct);

        string? ssoSignoutUrl = null;
        var identityProvider = AuthHelpers.GetClaim(user, ClaimConstants.ExternalIdentityProvider);
        var tenantId = AuthHelpers.GetClaim(user, ClaimConstants.ExternalTenantId);

        if (!string.IsNullOrEmpty(identityProvider) && !string.IsNullOrEmpty(tenantId))
        {
            var ssoJwt = AuthHelpers.GetClaim(user, ClaimConstants.SsoJwt);
            var oktaAuthority = AuthHelpers.GetClaim(user, ClaimConstants.OktaAuthority);
            var baseUrl = signOutBaseUrl ?? string.Empty;

            ssoSignoutUrl = identityProvider switch
            {
                "Microsoft" => $"https://login.microsoftonline.com/{tenantId}/oauth2/logout"
                               + $"?post_logout_redirect_uri={baseUrl}/sso-signed-out",
                "Okta" when !string.IsNullOrEmpty(oktaAuthority)
                    => $"{oktaAuthority}/oauth2/v1/logout"
                       + $"?id_token_hint={ssoJwt}&post_logout_redirect_uri={baseUrl}/SignedOut",
                _ => null
            };
        }

        logger.LogInformation("Logout completed for user '{LoginId}'", loginId);
        return Results.Ok(new { loggedOut = true, ssoSignoutUrl });
    }

    // ── Token refresh (Identity Server proxy) ─────────────────────────────────

    public async Task<IResult> ConnectAsync(ConnectRequest request, CancellationToken ct)
    {
        try
        {
            var result = await externalAuth.RefreshTokenAsync(request.RefreshToken, ct);
            return Results.Ok(result);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Token refresh failed");
            return Results.Problem(
                detail: "Token refresh failed. The refresh token may be expired or invalid.",
                statusCode: StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during token refresh");
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ── Agency / Witness login (§4.4) ─────────────────────────────────────────

    public async Task<IResult> AgencyLoginAsync(
        AgencyLoginRequest request,
        string ipAddress,
        string userAgent,
        string? deviceHash,
        CancellationToken ct)
    {
        VerifyUserResult baseResult;

        if (request.Login.IsExternalLogin && !string.IsNullOrEmpty(request.Login.UserName))
        {
            Guid.TryParse(request.Login.ExternalTenantId, out var tenantId);
            baseResult = await userBusiness.VerifyExternalUserAsync(
                request.Login.UserName, ipAddress, userAgent,
                request.Login.Loc ?? string.Empty, deviceHash,
                false, tenantId, null, ct);
        }
        else
        {
            baseResult = await userBusiness.VerifyUserAsync(
                request.Login.UserName, request.Login.EmailPassword ?? string.Empty,
                ipAddress, userAgent, request.Login.Loc, deviceHash,
                allowAgency: true, ct: ct);
        }

        if (!baseResult.IsSuccess || baseResult.User is null)
            return Results.Problem(
                detail: baseResult.Validation ?? "Authentication failed.",
                statusCode: StatusCodes.Status401Unauthorized);

        var agencyUser = await userBusiness.AgencyUserLoginAsync(
            baseResult.User, request.AgencyUser,
            ipAddress, userAgent, request.Login.Loc,
            request.IsWitnessUser, ct);

        if (agencyUser is null)
            return Results.Problem(detail: "UnauthorizedAccessFacility", statusCode: StatusCodes.Status403Forbidden);

        if (request.Login.IsExternalLogin)
        {
            agencyUser.IsExternalLogin = true;
            if (Guid.TryParse(request.Login.ExternalTenantId, out var tid))
                agencyUser.ExternalTenantId = tid;
        }

        var (response, _) = await responseBuilder.BuildAsync(
            agencyUser, ipAddress, request.Login.Loc, deviceHash, ct: ct);

        logger.LogInformation("Agency login successful for '{Username}'", request.Login.UserName);
        return Results.Ok(response);
    }

    // ── Verify PIN + issue token (§4.5) ───────────────────────────────────────

    public async Task<IResult> VerifyPinAsync(
        VerifyPinRequest request,
        string ipAddress,
        string userAgent,
        string? deviceHash,
        CancellationToken ct)
    {
        var user = await userBusiness.GetUserByUserIdAsync(request.UserName, ct);
        if (user is null)
            return Results.Problem(detail: "UserNotFound", statusCode: StatusCodes.Status401Unauthorized);

        if (user.Status != UserStatus.Active)
            return Results.Problem(detail: "UserAccountInactive", statusCode: StatusCodes.Status403Forbidden);

        var (isValid, failedCount) = await userBusiness.VerifyPinUserAsync(request.UserName, request.Pin, ct);

        if (!isValid)
        {
            failedCount++;
            await userBusiness.UpdateLogForVerifyPinAsync(
                request.UserName, string.Empty, ipAddress, userAgent, request.Loc, failedCount, ct);

            const int pinFailedMax = 3;
            if (failedCount < pinFailedMax)
                return Results.Problem(
                    detail: "InvalidPin",
                    statusCode: StatusCodes.Status401Unauthorized,
                    extensions: new Dictionary<string, object?> { ["attemptsLeft"] = pinFailedMax - failedCount });

            return Results.Problem(detail: "PinLocked", statusCode: StatusCodes.Status401Unauthorized);
        }

        if (failedCount > 0)
            await userBusiness.UpdateLogForVerifyPinAsync(
                request.UserName, string.Empty, ipAddress, userAgent, request.Loc, 0, ct);

        if (UserRoleCodes.AgencyAndWitnessRoles.Contains(user.RoleCode))
            return Results.Ok(new LoginResponse { RequireAgencyStep = true, RoleCodeForAgency = user.RoleCode });

        var (response, _) = await responseBuilder.BuildAsync(
            user, ipAddress, request.Loc, deviceHash,
            checkPasswordBreached: true, ct: ct);

        logger.LogInformation("PIN verified and token issued for '{Username}'", request.UserName);
        return Results.Ok(response);
    }

    // ── Register trusted device (§4.13) ──────────────────────────────────────

    public async Task<IResult> RegisterDeviceAsync(
        RegisterDeviceRequest request,
        string ipAddress,
        string userAgent,
        Guid? sessionId,
        CancellationToken ct)
    {
        var result = await userBusiness.RegisterDeviceAsync(
            request.Login, request.Pin, ipAddress, userAgent, sessionId, request.Loc, ct);

        if (result.IsError)
            return Results.Problem(detail: result.Result, statusCode: StatusCodes.Status400BadRequest);

        logger.LogInformation("Device registered for login '{Login}'", request.Login);
        return Results.Ok(new { deviceHash = result.DeviceHash, message = "DeviceRegisteredSuccessfully" });
    }

    // ── Agency / witness lookups (§4.20–4.22) ────────────────────────────────

    public async Task<IResult> AgencyLookupAsync(
        string registrationNumber, string lastName, bool isAgencyAIN, CancellationToken ct)
    {
        IEnumerable<string> roleCodes = isAgencyAIN
            ? [UserRoleCodes.FAgencyAIN]
            : UserRoleCodes.AgencyRoles;

        var result = await userBusiness.LookupAgencyUserAsync(registrationNumber, lastName, roleCodes, ct);
        return Results.Ok(result);
    }

    public async Task<IResult> ValidateAgencyRegistrationAsync(string registrationNumber, CancellationToken ct)
    {
        var result = await userBusiness.ValidateRegistrationNumberAsync(
            registrationNumber, UserRoleCodes.FAgencyRegisteredNurse, ct);
        return Results.Ok(result);
    }

    public async Task<IResult> WitnessLookupAsync(string doB, string lastName, CancellationToken ct)
    {
        var result = await userBusiness.LookupAgencyUserAsync(doB, lastName, [UserRoleCodes.FWitness], ct);
        return Results.Ok(result);
    }

    // ── Standalone PIN verify (§4.23) ─────────────────────────────────────────

    public async Task<IResult> PinVerifyStandaloneAsync(PinVerifyStandaloneRequest request, CancellationToken ct)
    {
        var (isValid, failedCount) = await userBusiness.VerifyPinUserAsync(request.UserName, request.Pin, ct);
        return Results.Ok(new { isValidPin = isValid, pinFailedCount = failedCount });
    }

    // ── Non-BESTMED pharmacist check (§4.25) ─────────────────────────────────

    public async Task<IResult> NonBestmedPharmacistCheckAsync(
        NonBestmedPharmacistCheckRequest request, string ipAddress, string userAgent, CancellationToken ct)
    {
        var show = await userBusiness.IsShowNonBESTmedPharmacistFirstLoginConfirmAsync(
            request.LoginId, request.EmailPassword,
            userAgent, ipAddress, request.Location ?? string.Empty, ct);

        return Results.Ok(new { show });
    }
}
