using System.Security.Claims;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

public interface IPasswordService
{
    Task<IResult> ChangeInitialPasswordAsync(ChangePasswordRequest request, ClaimsPrincipal user, string ipAddress, string userAgent, CancellationToken ct);
    Task<IResult> ChangeExpiredPasswordAsync(ChangePasswordRequest request, ClaimsPrincipal user, string ipAddress, string userAgent, CancellationToken ct);
    Task<IResult> RequestPasswordResetAsync(RequestPasswordResetRequest request, ClaimsPrincipal user, string ipAddress, string userAgent, string? deviceHash, CancellationToken ct);
    Task<IResult> ValidateResetTokenAsync(string token, string ipAddress, string? deviceHash, CancellationToken ct);
    Task<IResult> ChangePasswordByTokenAsync(ChangePasswordByTokenRequest request, ClaimsPrincipal user, string ipAddress, string userAgent, CancellationToken ct);
    Task<IResult> ResetPasswordSmsSendAsync(ResetPasswordSmsSendRequest request, string ipAddress, string userAgent, string? deviceHash, CancellationToken ct);
    Task<IResult> ResetPasswordBySmsAsync(ResetPasswordBySmsRequest request, string ipAddress, string userAgent, string? deviceHash, CancellationToken ct);
    Task<IResult> GeneratePasswordResetCodeAsync(string loginId, CancellationToken ct);
}
