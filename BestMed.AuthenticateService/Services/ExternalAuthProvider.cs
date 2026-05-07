using System.Net.Http.Json;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

public sealed class ExternalAuthProvider(HttpClient httpClient, ILogger<ExternalAuthProvider> logger) : IExternalAuthProvider
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Calling external auth API for login: {Username}", request.Username);

        var response = await httpClient.PostAsJsonAsync("/auth/login", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken)
            ?? throw new InvalidOperationException("External auth API returned an empty response.");

        logger.LogInformation("Login successful for {Username}, token expires at {ExpiresAt}", request.Username, result.ExpiresAt);
        return result;
    }

    public async Task LogoutAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Calling external auth API for logout");

        var request = new HttpRequestMessage(HttpMethod.Post, "/auth/logout");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        logger.LogInformation("Logout successful");
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Calling external auth API to refresh token");

        var response = await httpClient.PostAsJsonAsync("/auth/refresh", new { refreshToken }, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken)
            ?? throw new InvalidOperationException("External auth API returned an empty response.");

        logger.LogInformation("Token refresh successful, new token expires at {ExpiresAt}", result.ExpiresAt);
        return result;
    }
}
