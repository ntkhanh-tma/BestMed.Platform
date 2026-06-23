using System.Security.Claims;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

public interface IAuthService
{
    Task<IResult> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<IResult> LogoutAsync(string token, ClaimsPrincipal user, string ipAddress, string userAgent, string? signOutBaseUrl, CancellationToken ct);
    Task<IResult> ConnectAsync(ConnectRequest request, CancellationToken ct);
    Task<IResult> AgencyLoginAsync(AgencyLoginRequest request, string ipAddress, string userAgent, string? deviceHash, CancellationToken ct);
    Task<IResult> VerifyPinAsync(VerifyPinRequest request, string ipAddress, string userAgent, string? deviceHash, CancellationToken ct);
    Task<IResult> RegisterDeviceAsync(RegisterDeviceRequest request, string ipAddress, string userAgent, Guid? sessionId, CancellationToken ct);
    Task<IResult> AgencyLookupAsync(string registrationNumber, string lastName, bool isAgencyAIN, CancellationToken ct);
    Task<IResult> ValidateAgencyRegistrationAsync(string registrationNumber, CancellationToken ct);
    Task<IResult> WitnessLookupAsync(string doB, string lastName, CancellationToken ct);
    Task<IResult> PinVerifyStandaloneAsync(PinVerifyStandaloneRequest request, CancellationToken ct);
    Task<IResult> NonBestmedPharmacistCheckAsync(NonBestmedPharmacistCheckRequest request, string ipAddress, string userAgent, CancellationToken ct);
}
