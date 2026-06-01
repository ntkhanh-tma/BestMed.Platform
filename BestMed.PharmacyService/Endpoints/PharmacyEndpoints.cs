using BestMed.PharmacyService.DTOs;
using BestMed.PharmacyService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.PharmacyService.Endpoints;

public static class PharmacyEndpoints
{
    public static IEndpointRouteBuilder MapPharmacyEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/pharmacies")
            .WithTags("Pharmacies")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", (Guid id, IPharmacyService svc, CancellationToken ct)
                => svc.GetByIdAsync(id, ct))
            .WithName("GetPharmacyById")
            .WithDescription("Get a single pharmacy by ID including facilities")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", ([AsParameters] PharmacyQueryParameters query, IPharmacyService svc, CancellationToken ct)
                => svc.QueryAsync(query, ct))
            .WithName("QueryPharmacies")
            .WithDescription("Search and filter pharmacies with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", (Guid id, [FromBody] UpdatePharmacyRequest request, IPharmacyService svc, CancellationToken ct)
                => svc.UpdateAsync(id, request, ct))
            .WithName("UpdatePharmacy")
            .WithDescription("Update a single pharmacy")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }
}

