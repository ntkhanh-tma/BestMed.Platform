using System.Text.Json;
using BestMed.AuthenticateService.Models;
using Microsoft.AspNetCore.DataProtection;

namespace BestMed.AuthenticateService.Services;

public sealed class ExternalAuthProvider(
    HttpClient httpClient,
    IDataProtectionProvider dataProtection,
    IConfiguration configuration,
    ILogger<ExternalAuthProvider> logger) : IExternalAuthProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly IDataProtector _protector = dataProtection.CreateProtector("BestMed.Auth.Tokens");

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Authenticating user {Username} via Identity Server", request.Username);

        var clientId = configuration["IdentityServer:ClientId"]
            ?? throw new InvalidOperationException("IdentityServer:ClientId is not configured.");
        var scope = configuration["IdentityServer:Scope"] ?? "api";

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["username"] = request.Username,
            ["password"] = request.Password,
            ["scope"] = scope
        };

        var response = await httpClient.PostAsync("/connect/token", new FormUrlEncodedContent(formData), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("Identity Server login failed for {Username}: {StatusCode} {Error}",
                request.Username, response.StatusCode, errorBody);

            throw new HttpRequestException(
                $"Identity Server authentication failed: {errorBody}",
                null,
                response.StatusCode);
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<IdentityTokenResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Identity Server returned an empty response.");

        logger.LogInformation("Login successful for {Username}, token expires in {ExpiresIn}s", request.Username, tokenResponse.ExpiresIn);

        return new AuthResponse
        {
            AccessToken = _protector.Protect(tokenResponse.AccessToken),
            RefreshToken = tokenResponse.RefreshToken is not null ? _protector.Protect(tokenResponse.RefreshToken) : null,
            ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
            TokenType = tokenResponse.TokenType,
            PasswordExpiredDayLeft = tokenResponse.PasswordExpiredDayLeft
        };
    }

    public Task LogoutAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Logout requested — discarding token (no server-side revocation)");

        // Per Identity Server docs: tokens are self-contained JWTs with no server-side revocation endpoint.
        // The client should discard the access token and refresh token.
        return Task.CompletedTask;
    }

    public async Task<AuthResponse> RefreshTokenAsync(string protectedRefreshToken, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Refreshing token via Identity Server");

        var refreshToken = _protector.Unprotect(protectedRefreshToken);

        var clientId = configuration["IdentityServer:ClientId"]
            ?? throw new InvalidOperationException("IdentityServer:ClientId is not configured.");

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = clientId,
            ["refresh_token"] = refreshToken
        };

        var response = await httpClient.PostAsync("/connect/token", new FormUrlEncodedContent(formData), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("Identity Server token refresh failed: {StatusCode} {Error}", response.StatusCode, errorBody);

            throw new HttpRequestException(
                $"Token refresh failed: {errorBody}",
                null,
                response.StatusCode);
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<IdentityTokenResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Identity Server returned an empty response.");

        logger.LogInformation("Token refresh successful, new token expires in {ExpiresIn}s", tokenResponse.ExpiresIn);

        return new AuthResponse
        {
            AccessToken = _protector.Protect(tokenResponse.AccessToken),
            RefreshToken = tokenResponse.RefreshToken is not null ? _protector.Protect(tokenResponse.RefreshToken) : null,
            ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
            TokenType = tokenResponse.TokenType,
            PasswordExpiredDayLeft = tokenResponse.PasswordExpiredDayLeft
        };
    }

    private sealed class IdentityTokenResponse
    {
        public required string AccessToken { get; init; }
        public string? RefreshToken { get; init; }
        public required string TokenType { get; init; }
        public required int ExpiresIn { get; init; }
        public int? PasswordExpiredDayLeft { get; init; }
    }
}
