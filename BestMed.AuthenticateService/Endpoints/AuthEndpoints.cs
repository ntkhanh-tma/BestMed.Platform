using BestMed.AuthenticateService.Models;
using BestMed.AuthenticateService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.AuthenticateService.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth").WithTags("Authentication");

        group.MapPost("/login", (LoginRequest req, IAuthService svc, CancellationToken ct)
                => svc.LoginAsync(req, ct))
            .WithName("Login")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/logout", (HttpContext ctx, IAuthService svc, CancellationToken ct) =>
            {
                var token = ctx.Request.Headers.Authorization.ToString()
                    .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                var signOutBaseUrl = ctx.RequestServices.GetService<IConfiguration>()?["SignOutBaseUrl"];
                return svc.LogoutAsync(
                    token, ctx.User,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx),
                    signOutBaseUrl, ct);
            })
            .WithName("Logout")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPost("/connect", (ConnectRequest req, IAuthService svc, CancellationToken ct)
                => svc.ConnectAsync(req, ct))
            .WithName("Connect")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/agency-login", (AgencyLoginRequest req, HttpContext ctx, IAuthService svc, CancellationToken ct)
                => svc.AgencyLoginAsync(req,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx),
                    AuthHelpers.GetDeviceHash(ctx), ct))
            .WithName("AgencyLogin")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/verify-pin", (VerifyPinRequest req, HttpContext ctx, IAuthService svc, CancellationToken ct)
                => svc.VerifyPinAsync(req,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx),
                    AuthHelpers.GetDeviceHash(ctx), ct))
            .WithName("VerifyPin")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/device/register", (RegisterDeviceRequest req, HttpContext ctx, IAuthService svc, CancellationToken ct)
                => svc.RegisterDeviceAsync(req,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx),
                    AuthHelpers.GetClaimGuid(ctx.User, ClaimConstants.UserId), ct))
            .WithName("RegisterDevice")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapGet("/agency/lookup",
                ([AsParameters] AgencyLookupQuery q, IAuthService svc, CancellationToken ct)
                => svc.AgencyLookupAsync(q.RegistrationNumber ?? string.Empty, q.LastName ?? string.Empty, q.IsAgencyAIN, ct))
            .WithName("AgencyLookup")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapGet("/agency/validate-registration",
                ([AsParameters] RegistrationNumberQuery q, IAuthService svc, CancellationToken ct)
                => svc.ValidateAgencyRegistrationAsync(q.RegistrationNumber ?? string.Empty, ct))
            .WithName("ValidateAgencyRegistration")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapGet("/witness/lookup",
                ([AsParameters] WitnessLookupQuery q, IAuthService svc, CancellationToken ct)
                => svc.WitnessLookupAsync(q.DoB ?? string.Empty, q.LastName ?? string.Empty, ct))
            .WithName("WitnessLookup")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPost("/pin/verify", (PinVerifyStandaloneRequest req, IAuthService svc, CancellationToken ct)
                => svc.PinVerifyStandaloneAsync(req, ct))
            .WithName("PinVerify")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapPost("/non-bestmed-pharmacist/first-login-check",
                (NonBestmedPharmacistCheckRequest req, HttpContext ctx, IAuthService svc, CancellationToken ct)
                => svc.NonBestmedPharmacistCheckAsync(req,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx), ct))
            .WithName("NonBestmedPharmacistFirstLoginCheck")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        return routes;
    }

    private sealed record AgencyLookupQuery(
        [property: FromQuery] string? RegistrationNumber,
        [property: FromQuery] string? LastName,
        [property: FromQuery] bool IsAgencyAIN = false);

    private sealed record RegistrationNumberQuery(
        [property: FromQuery] string? RegistrationNumber);

    private sealed record WitnessLookupQuery(
        [property: FromQuery] string? DoB,
        [property: FromQuery] string? LastName);
}
