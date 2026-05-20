using BestMed.Common.Models;
using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.PrescriberService.Data;
using BestMed.PrescriberService.DTOs;
using BestMed.PrescriberService.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BestMed.PrescriberService.Endpoints;

public static class PrescriberEndpoints
{
    public static IEndpointRouteBuilder MapPrescriberEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/prescribers")
            .WithTags("Prescribers")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetPrescriberById")
            .WithDescription("Get a single prescriber by ID")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", QueryAsync)
            .WithName("QueryPrescribers")
            .WithDescription("Search and filter prescribers with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdatePrescriber")
            .WithDescription("Update a single prescriber")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        ReadOnlyPrescriberDbContext db,
        ILogger<PrescriberDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var prescriber = await db.Prescribers
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

    private static async Task<IResult> QueryAsync(
        [AsParameters] PrescriberQueryParameters query,
        ReadOnlyPrescriberDbContext db,
        ILogger<PrescriberDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var queryable = db.Prescribers
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

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdatePrescriberRequest request,
        PrescriberDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        ILogger<PrescriberDbContext> logger,
        CancellationToken cancellationToken)
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
