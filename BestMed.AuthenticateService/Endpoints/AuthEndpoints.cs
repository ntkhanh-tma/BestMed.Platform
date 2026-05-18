using BestMed.AuthenticateService.Models;
using BestMed.AuthenticateService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BestMed.AuthenticateService.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth")
            .WithTags("Authentication");

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithDescription("Authenticate a user via the external auth provider and return a JWT token")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .WithDescription("Revoke the current access token")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPost("/connect", ConnectAsync)
            .WithName("Connect")
            .WithDescription("Refresh an expired access token using a refresh token")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        return routes;
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        IExternalAuthProvider authProvider,
        ILogger<IExternalAuthProvider> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Login attempt for user '{Username}'", request.Username);

        try
        {
            var result = await authProvider.LoginAsync(request, cancellationToken);
            logger.LogInformation("Login successful for user '{Username}'", request.Username);
            return Results.Ok(result);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Login failed for user '{Username}' — authentication rejected", request.Username);
            return Results.Problem(
                detail: "Authentication failed. Please check your credentials.",
                statusCode: StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for user '{Username}'", request.Username);
            return Results.Problem(
                detail: "An unexpected error occurred during authentication.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> LogoutAsync(
        HttpContext httpContext,
        IExternalAuthProvider authProvider,
        ILogger<IExternalAuthProvider> logger,
        CancellationToken cancellationToken)
    {
        var token = httpContext.Request.Headers.Authorization
            .ToString()
            .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Results.BadRequest("No access token provided.");
        }

        try
        {
            await authProvider.LogoutAsync(token, cancellationToken);
            logger.LogInformation("Logout completed successfully");
            return Results.Ok();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Logout failed — upstream auth provider returned an error");
            return Results.Problem(
                detail: "Logout failed.",
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during logout");
            return Results.Problem(
                detail: "An unexpected error occurred during logout.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> ConnectAsync(
        [FromBody] ConnectRequest request,
        IExternalAuthProvider authProvider,
        ILogger<IExternalAuthProvider> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Token refresh attempt");

        try
        {
            var result = await authProvider.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            logger.LogInformation("Token refresh successful");
            return Results.Ok(result);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Token refresh failed — refresh token may be expired or invalid");
            return Results.Problem(
                detail: "Token refresh failed. The refresh token may be expired or invalid.",
                statusCode: StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during token refresh");
            return Results.Problem(
                detail: "An unexpected error occurred during token refresh.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
