using BestMed.PrescriberService.DTOs;
using BestMed.PrescriberService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.PrescriberService.Endpoints;

public static class PrescriberEndpoints
{
    public static IEndpointRouteBuilder MapPrescriberEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/prescribers")
            .WithTags("Prescribers")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", (Guid id, IPrescriberService svc, CancellationToken ct)
                => svc.GetByIdAsync(id, ct))
            .WithName("GetPrescriberById")
            .WithDescription("Get a single prescriber by ID")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", ([AsParameters] PrescriberQueryParameters query, IPrescriberService svc, CancellationToken ct)
                => svc.QueryAsync(query, ct))
            .WithName("QueryPrescribers")
            .WithDescription("Search and filter prescribers with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", (Guid id, [FromBody] UpdatePrescriberRequest request, IPrescriberService svc, CancellationToken ct)
                => svc.UpdateAsync(id, request, ct))
            .WithName("UpdatePrescriber")
            .WithDescription("Update a single prescriber")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }
}
