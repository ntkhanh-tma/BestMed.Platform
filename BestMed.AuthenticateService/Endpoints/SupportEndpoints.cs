using BestMed.AuthenticateService.Services;

namespace BestMed.AuthenticateService.Endpoints;

public static class SupportEndpoints
{
    public static IEndpointRouteBuilder MapSupportEndpoints(this IEndpointRouteBuilder routes)
    {
        var authGroup = routes.MapGroup("/auth").WithTags("Support");

        authGroup.MapGet("/support",
                (ISupportService svc, CancellationToken ct) => svc.GetSupportHtmlAsync(ct))
            .WithName("GetSupportHtml")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitLight);

        authGroup.MapGet("/support/info",
                (ISupportService svc, CancellationToken ct) => svc.GetSupportInfoAsync(ct))
            .WithName("GetSupportInfo")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitLight);

        authGroup.MapGet("/server-name",
                (ISupportService svc) => svc.GetServerName())
            .WithName("GetServerName")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitLight);

        routes.MapPost("/OAuth2/Token", async (HttpContext ctx, ISupportService svc, CancellationToken ct) =>
            {
                if (!ctx.Request.HasFormContentType)
                    return Results.BadRequest("Request must be application/x-www-form-urlencoded.");
                var form = await ctx.Request.ReadFormAsync(ct);
                return await svc.OAuth2TokenAsync(form, ct);
            })
            .WithTags("OAuth2")
            .WithName("OAuth2Token")
            .AllowAnonymous()
            .RequireRateLimiting(Extensions.RateLimitHeavy)
            .Accepts<IFormCollection>("application/x-www-form-urlencoded");

        return routes;
    }
}
