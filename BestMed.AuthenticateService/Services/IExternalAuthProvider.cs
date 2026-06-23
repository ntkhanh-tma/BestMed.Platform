using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

public interface IExternalAuthProvider
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<string> GetClientCredentialsTokenRawAsync(IFormCollection form, CancellationToken cancellationToken = default);
}
