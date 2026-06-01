using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.Common.Models;
using BestMed.PharmacyService.Data;
using BestMed.PharmacyService.DTOs;
using BestMed.PharmacyService.Mapping;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.PharmacyService.Services;

/// <summary>
/// Implements all pharmacy business/data logic.
/// Dependencies are injected so the class is fully unit-testable in isolation.
/// </summary>
public sealed class PharmacyService(
    PharmacyDbContext db,
    ReadOnlyPharmacyDbContext readDb,
    IOutputCacheStore cache,
    IEventPublisher eventPublisher,
    ILogger<PharmacyService> logger) : IPharmacyService
{
    public async Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var pharmacy = await readDb.Pharmacies
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

    public async Task<IResult> QueryAsync(PharmacyQueryParameters query, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = readDb.Pharmacies
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

    public async Task<IResult> UpdateAsync(Guid id, UpdatePharmacyRequest request, CancellationToken cancellationToken)
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
