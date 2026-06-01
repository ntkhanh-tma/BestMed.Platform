using BestMed.FacilityService.DTOs;
using BestMed.FacilityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.FacilityService.Endpoints;

public static class FacilityEndpoints
{
    public static IEndpointRouteBuilder MapFacilityEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/facilities")
            .WithTags("Facilities")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", (Guid id, IFacilityService svc, CancellationToken ct)
                => svc.GetByIdAsync(id, ct))
            .WithName("GetFacilityById")
            .WithDescription("Get a single facility by ID including sections")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", ([AsParameters] FacilityQueryParameters query, IFacilityService svc, CancellationToken ct)
                => svc.QueryAsync(query, ct))
            .WithName("QueryFacilities")
            .WithDescription("Search and filter facilities with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", (Guid id, [FromBody] UpdateFacilityRequest request, IFacilityService svc, CancellationToken ct)
                => svc.UpdateAsync(id, request, ct))
            .WithName("UpdateFacility")
            .WithDescription("Update a single facility")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }
}

