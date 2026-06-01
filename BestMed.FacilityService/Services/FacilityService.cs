using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.Common.Models;
using BestMed.FacilityService.Data;
using BestMed.FacilityService.DTOs;
using BestMed.FacilityService.Mapping;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.FacilityService.Services;

/// <summary>
/// Implements all facility business/data logic.
/// Dependencies are injected so the class is fully unit-testable in isolation.
/// </summary>
public sealed class FacilityService(
    FacilityDbContext db,
    ReadOnlyFacilityDbContext readDb,
    IOutputCacheStore cache,
    IEventPublisher eventPublisher,
    ILogger<FacilityService> logger) : IFacilityService
{
    public async Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var facility = await readDb.Facilities
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

    public async Task<IResult> QueryAsync(FacilityQueryParameters query, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = readDb.Facilities
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

    public async Task<IResult> UpdateAsync(Guid id, UpdateFacilityRequest request, CancellationToken cancellationToken)
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
