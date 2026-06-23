using System.Security.Claims;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

internal sealed class SsoService(
    IUserBusiness userBusiness,
    LoginResponseBuilder responseBuilder,
    IConfiguration configuration,
    ILogger<SsoService> logger) : ISsoService
{
    // ── SSO provider discovery (§4.2) ─────────────────────────────────────────

    public async Task<IResult> DiscoverAsync(string userName, string ipAddress, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Results.BadRequest(new { error = "UserNameRequired" });

        var ssoResult = await userBusiness.GetSSOProviderAsync(userName, ct);

        await userBusiness.CreateSSOLoginLogAsync(
            userName, SSOLoginLogType.SSOLookup, ssoResult.Log,
            ipAddress, ssoResult.ProviderType != SsoProviderType.None, ct);

        if (!string.IsNullOrEmpty(ssoResult.ErrorCode))
        {
            if (ssoResult.ErrorCode == "UserIsLockedNoReset")
                return Results.Problem(detail: "UserIsLockedNoReset", statusCode: StatusCodes.Status403Forbidden);

            return Results.Ok(new { provider = (string?)null, error = "SSONotAvailable" });
        }

        string? redirectUrl = ssoResult.ProviderType switch
        {
            SsoProviderType.Microsoft => BuildMicrosoftRedirectUrl(ssoResult.Provider, userName),
            SsoProviderType.Okta => BuildOktaRedirectUrl(ssoResult.Provider, userName),
            _ => null
        };

        if (redirectUrl is null)
            return Results.Ok(new { provider = (string?)null, error = "SSONotAvailable" });

        return Results.Ok(new { provider = ssoResult.ProviderType.ToString(), redirectUrl });
    }

    // ── SSO OIDC callback (§4.3) ──────────────────────────────────────────────

    public async Task<IResult> CallbackAsync(
        ClaimsPrincipal externalUser,
        string ipAddress,
        string userAgent,
        string? deviceHash,
        CancellationToken ct)
    {
        var tenantId = AuthHelpers.GetClaim(externalUser, ClaimConstants.ExternalTenantId);
        var extUserId = AuthHelpers.GetClaim(externalUser, ClaimConstants.ExternalUserId);
        var extUserRole = externalUser.FindFirst("EXTERNAL_USER_ROLE")?.Value?.Trim();

        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(extUserId))
            return Results.Problem(detail: "MissingExternalClaims", statusCode: StatusCodes.Status401Unauthorized);

        var isRBAC = Guid.TryParse(tenantId, out var facilityGroupId)
                     && await userBusiness.UserRBACEnableAsync(facilityGroupId, ct);

        var result = await userBusiness.VerifyExternalUserAsync(
            extUserId, ipAddress, userAgent, string.Empty,
            deviceHash, isRBAC, facilityGroupId, extUserRole, ct);

        if (!result.IsSuccess || result.User is null)
        {
            await userBusiness.CreateSSOLoginLogAsync(extUserId, SSOLoginLogType.SSOLogin, null, ipAddress, false, ct);
            return Results.Problem(detail: result.Validation, statusCode: StatusCodes.Status401Unauthorized);
        }

        var user = result.User;

        if (isRBAC && !string.IsNullOrEmpty(extUserRole))
        {
            if (!await userBusiness.UserRoleTemplateValidAsync(facilityGroupId, extUserRole, ct))
                return Results.Problem(detail: "InvalidRoleName", statusCode: StatusCodes.Status403Forbidden);

            var facilityIdList = await ResolveSSOFacilityListAsync(
                externalUser, facilityGroupId, extUserRole, ct);

            if (facilityIdList.Count == 0)
                return Results.Problem(detail: "UnauthorizedAccessContactUs", statusCode: StatusCodes.Status403Forbidden);

            MapExternalUserProfile(user, externalUser);

            if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName)
                || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.ContactNumber))
                return Results.BadRequest(new { error = "AccountIsIncomplete" });

            user = await userBusiness.ApplyExternalUserRoleAsync(
                user, facilityGroupId, extUserRole, facilityIdList, ct);
            user.OriginalId = user.Id;

            if (!await userBusiness.ValidateUserGeolocationAsync(user, ipAddress, facilityIdList, ct))
                return Results.Problem(detail: "GeolocationNotAllowed", statusCode: StatusCodes.Status403Forbidden);
        }

        if (UserRoleCodes.AgencyAndWitnessRoles.Contains(user.RoleCode))
        {
            await userBusiness.CreateSSOLoginLogAsync(extUserId, SSOLoginLogType.SSOLogin, null, ipAddress, true, ct);
            return Results.Ok(new { requireAgencyStep = true, externalLogin = true, tenantId, extUserId });
        }

        if (Guid.TryParse(tenantId, out var tid))
            user.ExternalTenantId = tid;

        var (response, _) = await responseBuilder.BuildAsync(user, ipAddress, null, deviceHash, ct: ct);

        await userBusiness.CreateSSOLoginLogAsync(extUserId, SSOLoginLogType.SSOLogin, null, ipAddress, true, ct);
        logger.LogInformation("SSO callback login successful for external user '{ExtUserId}'", extUserId);

        return Results.Ok(response);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private string? BuildMicrosoftRedirectUrl(string? provider, string loginHint)
    {
        var baseUrl = configuration["IdentityServer:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(provider)) return null;

        return $"{baseUrl}/connect/authorize"
               + $"?client_id={configuration["IdentityServer:ClientId"]}"
               + $"&response_type=code&scope=openid profile"
               + $"&login_hint={Uri.EscapeDataString(loginHint)}"
               + $"&acr_values=tenant:{provider}";
    }

    private string? BuildOktaRedirectUrl(string? provider, string loginHint)
    {
        var oktaSettings = configuration.GetSection($"OktaApps:{provider}");
        if (!oktaSettings.Exists())
        {
            logger.LogWarning("No Okta app settings found for provider '{Provider}'", provider);
            return null;
        }

        return $"{oktaSettings["AuthorizeUrl"]}"
               + $"?client_id={oktaSettings["ClientId"]}"
               + $"&response_type=code&scope=openid profile email"
               + $"&redirect_uri={Uri.EscapeDataString(oktaSettings["RedirectUri"] ?? string.Empty)}"
               + $"&login_hint={Uri.EscapeDataString(loginHint)}";
    }

    private async Task<List<Guid>> ResolveSSOFacilityListAsync(
        ClaimsPrincipal claims, Guid facilityGroupId, string extUserRole, CancellationToken ct)
    {
        var isAgencyNDIS = extUserRole == UserRoleCodes.FAgencyNDISWorker;

        var orgIdList = claims.FindFirst("EXTERNAL_USER_ORGANISATION_ID_LIST")?.Value;
        if (!string.IsNullOrEmpty(orgIdList))
        {
            var ids = orgIdList.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Guid.TryParse(s.Trim(), out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();

            if (ids.Count > 0)
                return await userBusiness.ValidateSSOFacilityIdAsync(facilityGroupId, ids, isAgencyNDIS, ct);
        }

        var orgNameList = claims.FindFirst("EXTERNAL_USER_ORGANISATION_NAME_LIST")?.Value;
        if (!string.IsNullOrEmpty(orgNameList))
        {
            var names = orgNameList.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();

            return await userBusiness.ValidateSSOFacilityNameAsync(facilityGroupId, names, isAgencyNDIS, ct);
        }

        return [];
    }

    private static void MapExternalUserProfile(UserBO user, ClaimsPrincipal claims)
    {
        user.FirstName = claims.FindFirst("EXTERNAL_USER_FIRSTNAME")?.Value ?? string.Empty;
        user.LastName = claims.FindFirst("EXTERNAL_USER_LASTNAME")?.Value ?? string.Empty;
        user.Email = claims.FindFirst("EXTERNAL_USER_EMAIL")?.Value ?? string.Empty;
        user.ContactNumber = claims.FindFirst("EXTERNAL_USER_CONTACTNUMBER")?.Value ?? string.Empty;
        user.AHPRANumber = claims.FindFirst("EXTERNAL_USER_AHPRANumber")?.Value ?? string.Empty;
        user.UserQualifications = claims.FindFirst("EXTERNAL_USER_QUALIFICATIONS")?.Value ?? string.Empty;
    }
}
