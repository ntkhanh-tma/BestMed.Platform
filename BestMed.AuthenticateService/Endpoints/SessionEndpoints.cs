using BestMed.AuthenticateService.Models;
using BestMed.AuthenticateService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.AuthenticateService.Endpoints;

public static class SessionEndpoints
{
    public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth").WithTags("Session");

        group.MapPost("/terms-and-conditions/accept",
                (HttpContext ctx, ISessionService svc, CancellationToken ct)
                => svc.AcceptTermsAsync(ctx.User, AuthHelpers.GetIpAddress(ctx), ct))
            .WithName("AcceptTermsAndConditions")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPost("/switch-organisation",
                ([FromBody] SwitchOrganisationRequest req, HttpContext ctx, ISessionService svc, CancellationToken ct)
                => svc.SwitchOrganisationAsync(req, ctx.User, ct))
            .WithName("SwitchOrganisation")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPost("/switch-section",
                ([FromBody] SwitchSectionRequest req, HttpContext ctx, ISessionService svc, CancellationToken ct)
                => svc.SwitchSectionAsync(req, ctx.User, ct))
            .WithName("SwitchSection")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPost("/switch-to-facility",
                ([FromBody] SwitchToFacilityRequest req, HttpContext ctx, ISessionService svc, CancellationToken ct)
                => svc.SwitchToFacilityAsync(req, ctx.User, ct))
            .WithName("SwitchToFacility")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPost("/switch-doctor-mode",
                (HttpContext ctx, ISessionService svc, CancellationToken ct)
                => svc.SwitchDoctorModeAsync(ctx.User, ct))
            .WithName("SwitchDoctorMode")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapGet("/user-facilities",
                (HttpContext ctx, ISessionService svc, CancellationToken ct)
                => svc.GetUserFacilitiesAsync(ctx.User, ct))
            .WithName("GetUserFacilities")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitLight);

        group.MapGet("/user-facilities/in-possession-report",
                (HttpContext ctx, ISessionService svc, CancellationToken ct)
                => svc.GetUserFacilitiesForReportAsync(ctx.User, ct))
            .WithName("GetUserFacilitiesForReport")
            .RequireAuthorization()
            .RequireRateLimiting(Extensions.RateLimitLight);

        return routes;
    }
}
