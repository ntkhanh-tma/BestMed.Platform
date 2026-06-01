using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.Common.Models;
using BestMed.PrescriberService.Data;
using BestMed.PrescriberService.DTOs;
using BestMed.PrescriberService.Mapping;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.PrescriberService.Services;

/// <summary>
/// Implements all prescriber business/data logic.
/// Dependencies are injected so the class is fully unit-testable in isolation.
/// </summary>
public sealed class PrescriberService(
    PrescriberDbContext db,
    ReadOnlyPrescriberDbContext readDb,
    IOutputCacheStore cache,
    IEventPublisher eventPublisher,
    ILogger<PrescriberService> logger) : IPrescriberService
{
    public async Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var prescriber = await readDb.Prescribers
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            return prescriber is null
                ? Results.NotFound()
                : Results.Ok(prescriber.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving prescriber {PrescriberId}", id);
            return Results.Problem("An error occurred while retrieving the prescriber.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> QueryAsync(PrescriberQueryParameters query, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = readDb.Prescribers
                .ApplyFilters(query)
                .ApplySorting(query);

            var totalCount = await queryable.CountAsync(cancellationToken);
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => p.ToDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(new PagedResponse<PrescriberDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error querying prescribers");
            return Results.Problem("An error occurred while querying prescribers.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> UpdateAsync(Guid id, UpdatePrescriberRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating prescriber {PrescriberId}", id);

        try
        {
            var prescriber = await db.Prescribers.FindAsync([id], cancellationToken);
            if (prescriber is null) return Results.NotFound();

            request.ApplyTo(prescriber);

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("prescribers", cancellationToken);

            await eventPublisher.PublishAsync(new PrescriberUpdatedEvent
            {
                PrescriberId = prescriber.Id,
                PrescriberName = prescriber.PrescriberName,
                PrescriberCode = prescriber.PrescriberCode
            }, cancellationToken);

            logger.LogInformation("Prescriber {PrescriberId} updated successfully", id);
            return Results.Ok(prescriber.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating prescriber {PrescriberId}", id);
            return Results.Problem("An error occurred while updating the prescriber.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
