using BestMed.AuthenticateService.Models;
using BestMed.AuthenticateService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

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
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authProvider.LoginAsync(request, cancellationToken);
            return Results.Ok(result);
        }
        catch (HttpRequestException)
        {
            return Results.Problem(
                detail: "Authentication failed. Please check your credentials.",
                statusCode: StatusCodes.Status401Unauthorized);
        }
    }

    private static async Task<IResult> LogoutAsync(
        HttpContext httpContext,
        IExternalAuthProvider authProvider,
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
            return Results.Ok();
        }
        catch (HttpRequestException)
        {
            return Results.Problem(
                detail: "Logout failed.",
                statusCode: StatusCodes.Status502BadGateway);
        }
    }

    private static async Task<IResult> ConnectAsync(
        [FromBody] ConnectRequest request,
        IExternalAuthProvider authProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authProvider.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            return Results.Ok(result);
        }
        catch (HttpRequestException)
        {
            return Results.Problem(
                detail: "Token refresh failed. The refresh token may be expired or invalid.",
                statusCode: StatusCodes.Status401Unauthorized);
        }
    }
}
