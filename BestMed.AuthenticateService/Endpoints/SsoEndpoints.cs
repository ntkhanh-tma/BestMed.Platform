using BestMed.AuthenticateService.Models;
using BestMed.AuthenticateService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.AuthenticateService.Endpoints;

public static class SsoEndpoints
{
    public static IEndpointRouteBuilder MapSsoEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth/sso").WithTags("SSO");

        group.MapPost("/discover",
                ([FromBody] SsoDiscoverRequest req, HttpContext ctx, ISsoService svc, CancellationToken ct)
                => svc.DiscoverAsync(req.UserName ?? string.Empty, AuthHelpers.GetIpAddress(ctx), ct))
            .WithName("SsoDiscover")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        group.MapGet("/callback",
                (HttpContext ctx, ISsoService svc, CancellationToken ct)
                => svc.CallbackAsync(
                    ctx.User,
                    AuthHelpers.GetIpAddress(ctx), AuthHelpers.GetUserAgent(ctx),
                    AuthHelpers.GetDeviceHash(ctx), ct))
            .WithName("SsoCallback")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        return routes;
    }
}
