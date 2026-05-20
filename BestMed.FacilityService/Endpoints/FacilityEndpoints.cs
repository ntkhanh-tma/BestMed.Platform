using BestMed.Common.Models;
using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.FacilityService.Data;
using BestMed.FacilityService.DTOs;
using BestMed.FacilityService.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.FacilityService.Endpoints;

public static class FacilityEndpoints
{
    public static IEndpointRouteBuilder MapFacilityEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/facilities")
            .WithTags("Facilities")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetFacilityById")
            .WithDescription("Get a single facility by ID including sections")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", QueryAsync)
            .WithName("QueryFacilities")
            .WithDescription("Search and filter facilities with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateFacility")
            .WithDescription("Update a single facility")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        ReadOnlyFacilityDbContext db,
        ILogger<FacilityDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var facility = await db.Facilities
                .Include(f => f.Sections)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

            return facility is null
                ? Results.NotFound()
                : Results.Ok(facility.ToDetailDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving facility {FacilityId}", id);
            return Results.Problem("An error occurred while retrieving the facility.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> QueryAsync(
        [AsParameters] FacilityQueryParameters query,
        ReadOnlyFacilityDbContext db,
        ILogger<FacilityDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var queryable = db.Facilities
                .ApplyFilters(query)
                .ApplySorting(query);

            var totalCount = await queryable.CountAsync(cancellationToken);
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(f => f.ToDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(new PagedResponse<FacilityDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error querying facilities");
            return Results.Problem("An error occurred while querying facilities.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateFacilityRequest request,
        FacilityDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        ILogger<FacilityDbContext> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating facility {FacilityId}", id);

        try
        {
            var facility = await db.Facilities.FindAsync([id], cancellationToken);
            if (facility is null) return Results.NotFound();

            request.ApplyTo(facility);

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("facilities", cancellationToken);

            await eventPublisher.PublishAsync(new FacilityUpdatedEvent
            {
                FacilityId = facility.Id,
                FacilityName = facility.Name
            }, cancellationToken);

            logger.LogInformation("Facility {FacilityId} updated successfully", id);
            return Results.Ok(facility.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating facility {FacilityId}", id);
            return Results.Problem("An error occurred while updating the facility.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
