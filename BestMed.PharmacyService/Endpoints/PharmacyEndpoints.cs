using BestMed.Common.Models;
using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.PharmacyService.Data;
using BestMed.PharmacyService.DTOs;
using BestMed.PharmacyService.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.PharmacyService.Endpoints;

public static class PharmacyEndpoints
{
    public static IEndpointRouteBuilder MapPharmacyEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/pharmacies")
            .WithTags("Pharmacies")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetPharmacyById")
            .WithDescription("Get a single pharmacy by ID including facilities")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", QueryAsync)
            .WithName("QueryPharmacies")
            .WithDescription("Search and filter pharmacies with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdatePharmacy")
            .WithDescription("Update a single pharmacy")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        ReadOnlyPharmacyDbContext db,
        ILogger<PharmacyDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var pharmacy = await db.Pharmacies
                .Include(p => p.Facilities)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            return pharmacy is null
                ? Results.NotFound()
                : Results.Ok(pharmacy.ToDetailDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving pharmacy {PharmacyId}", id);
            return Results.Problem("An error occurred while retrieving the pharmacy.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> QueryAsync(
        [AsParameters] PharmacyQueryParameters query,
        ReadOnlyPharmacyDbContext db,
        ILogger<PharmacyDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var queryable = db.Pharmacies
                .ApplyFilters(query)
                .ApplySorting(query);

            var totalCount = await queryable.CountAsync(cancellationToken);
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => p.ToDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(new PagedResponse<PharmacyDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error querying pharmacies");
            return Results.Problem("An error occurred while querying pharmacies.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdatePharmacyRequest request,
        PharmacyDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        ILogger<PharmacyDbContext> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating pharmacy {PharmacyId}", id);

        try
        {
            var pharmacy = await db.Pharmacies.FindAsync([id], cancellationToken);
            if (pharmacy is null) return Results.NotFound();

            request.ApplyTo(pharmacy);

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("pharmacies", cancellationToken);

            await eventPublisher.PublishAsync(new PharmacyUpdatedEvent
            {
                PharmacyId = pharmacy.Id,
                PharmacyName = pharmacy.Name
            }, cancellationToken);

            logger.LogInformation("Pharmacy {PharmacyId} updated successfully", id);
            return Results.Ok(pharmacy.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating pharmacy {PharmacyId}", id);
            return Results.Problem("An error occurred while updating the pharmacy.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
