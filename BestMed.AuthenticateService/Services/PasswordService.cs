using System.Security.Claims;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

internal sealed class PasswordService(
    IUserBusiness userBusiness,
    IJwtTokenService jwtService,
    LoginResponseBuilder responseBuilder,
    ILogger<PasswordService> logger) : IPasswordService
{
    private readonly ILogger<PasswordService> _logger = logger;
    // ── Change initial password (§4.8) ────────────────────────────────────────

    public async Task<IResult> ChangeInitialPasswordAsync(
        ChangePasswordRequest request, ClaimsPrincipal user, string ipAddress, string userAgent, CancellationToken ct)
    {
        if (request.NewPassword != request.ReTypePassword)
            return Results.BadRequest(new { error = "PasswordMismatch" });

        var userId = AuthHelpers.GetClaimGuid(user, ClaimConstants.UserId);
        if (userId is null)
            return Results.Problem(detail: "InvalidToken", statusCode: StatusCodes.Status401Unauthorized);

        var error = await userBusiness.ChangeInitialPasswordAsync(
            request.NewPassword, ipAddress, userAgent, request.Loc, userId.Value, ct);

        if (!string.IsNullOrEmpty(error))
            return Results.BadRequest(new { error });

        var updatedToken = jwtService.UpdateClaims(
            user, new Dictionary<string, string[]> { [ClaimConstants.IsInitialLogin] = ["False"] });

        return Results.Ok(new { changed = true, accessToken = updatedToken });
    }

    // ── Change expired password (§4.9) ────────────────────────────────────────

    public async Task<IResult> ChangeExpiredPasswordAsync(
        ChangePasswordRequest request, ClaimsPrincipal user, string ipAddress, string userAgent, CancellationToken ct)
    {
        if (request.NewPassword != request.ReTypePassword)
            return Results.BadRequest(new { error = "PasswordMismatch" });

        var userId = AuthHelpers.GetClaimGuid(user, ClaimConstants.UserId);
        if (userId is null)
            return Results.Problem(detail: "InvalidToken", statusCode: StatusCodes.Status401Unauthorized);

        var error = await userBusiness.ChangeExpiredPasswordAsync(
            request.NewPassword, ipAddress, userAgent, request.Loc, userId.Value, ct);

        if (!string.IsNullOrEmpty(error))
            return Results.BadRequest(new { error });

        var updatedToken = jwtService.UpdateClaims(user, new Dictionary<string, string[]>
        {
            [ClaimConstants.IsPasswordExpired] = ["False"],
            [ClaimConstants.PasswordDaysLeft] = [int.MaxValue.ToString()]
        });

        return Results.Ok(new { changed = true, accessToken = updatedToken });
    }

    // ── Request password reset by email (§4.10) ───────────────────────────────

    public async Task<IResult> RequestPasswordResetAsync(
        RequestPasswordResetRequest request, ClaimsPrincipal user,
        string ipAddress, string userAgent, string? deviceHash, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.EmailId) || request.EmailId.Contains('/'))
            return Results.BadRequest(new { error = "InvalidEmailId" });

        if (await userBusiness.IsDoctorLoginWithValidMobileNumberAsync(request.EmailId, ct))
            return Results.Ok(new { requireSmsReset = true });

        var sessionId = AuthHelpers.GetClaimGuid(user, ClaimConstants.UserId);

        var result = await userBusiness.ResetPasswordAsync(
            request.EmailId, ipAddress, userAgent, sessionId, request.Loc, deviceHash, ct);

        if (result.IsError)
            return Results.BadRequest(new { error = result.Result });

        var masked = LoginResponseBuilder.MaskEmail(result.Email ?? request.EmailId);
        return Results.Ok(new { message = $"Link sent to {masked}" });
    }

    // ── Validate reset token (GET §4.11) ──────────────────────────────────────

    public async Task<IResult> ValidateResetTokenAsync(string token, string ipAddress, string? deviceHash, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Results.BadRequest(new { error = "InvalidToken" });

        var errorMessage = await userBusiness.CheckExpiredResetPasswordTokenAsync(token, ipAddress, null, deviceHash, ct);
        return Results.Ok(new { valid = true, errorMessage });
    }

    // ── Change password by token (POST §4.11) ─────────────────────────────────

    public async Task<IResult> ChangePasswordByTokenAsync(
        ChangePasswordByTokenRequest request, ClaimsPrincipal user,
        string ipAddress, string userAgent, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.ResetCode))
            return Results.BadRequest(new { error = "InvalidToken" });

        if (request.NewPassword != request.ReTypePassword)
            return Results.BadRequest(new { error = "PasswordMismatch" });

        var sessionId = AuthHelpers.GetClaimGuid(user, ClaimConstants.UserId);

        var error = await userBusiness.ChangePasswordByTokenAsync(
            request.ResetCode, request.NewPassword, ipAddress, userAgent, sessionId, ct);

        if (!string.IsNullOrEmpty(error))
            return Results.BadRequest(new { error });

        return Results.Ok(new { changed = true });
    }

    // ── Send reset SMS (§4.12) ────────────────────────────────────────────────

    public async Task<IResult> ResetPasswordSmsSendAsync(
        ResetPasswordSmsSendRequest request, string ipAddress, string userAgent, string? deviceHash, CancellationToken ct)
    {
        var result = await userBusiness.ResetPasswordAsync(
            request.EmailId, ipAddress, userAgent, null, request.Loc, deviceHash, ct);

        if (result.IsError)
            return Results.BadRequest(new { error = result.Result });

        return Results.Ok(new { message = LoginResponseBuilder.MaskEmail(result.Email ?? request.EmailId) });
    }

    // ── Verify SMS code + reset + auto-login (§4.12) ──────────────────────────

    public async Task<IResult> ResetPasswordBySmsAsync(
        ResetPasswordBySmsRequest request, string ipAddress, string userAgent, string? deviceHash, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.MobileNumber))
            return Results.BadRequest(new { error = "MobileNumberRequired" });

        if (request.NewPassword != request.ReTypePassword)
            return Results.BadRequest(new { error = "PasswordMismatch" });

        var error = await userBusiness.ResetPasswordBySmsAsync(
            request.EmailId, request.SmsCode, request.NewPassword, ipAddress, userAgent, request.Loc, ct);

        if (!string.IsNullOrEmpty(error))
            return Results.BadRequest(new { error });

        var verifyResult = await userBusiness.VerifyUserAsync(
            request.EmailId, request.NewPassword, ipAddress, userAgent, request.Loc, deviceHash, ct: ct);

        if (!verifyResult.IsSuccess || verifyResult.User is null)
            return Results.Problem(detail: verifyResult.Validation, statusCode: StatusCodes.Status401Unauthorized);

        var (response, _) = await responseBuilder.BuildAsync(
            verifyResult.User, ipAddress, request.Loc, deviceHash, ct: ct);

        return Results.Ok(response);
    }

    // ── Generate password reset verification code (§4.24) ─────────────────────

    public async Task<IResult> GeneratePasswordResetCodeAsync(string loginId, CancellationToken ct)
    {
        var result = await userBusiness.GeneratePasswordResetVerificationCodeAsync(loginId, ct);
        return Results.Ok(result);
    }
}
