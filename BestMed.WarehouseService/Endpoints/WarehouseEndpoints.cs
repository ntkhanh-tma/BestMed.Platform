using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.Common.Models;
using BestMed.WarehouseService.Data;
using BestMed.WarehouseService.DTOs;
using BestMed.WarehouseService.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BestMed.WarehouseService.Endpoints;

public static class WarehouseEndpoints
{
    public static IEndpointRouteBuilder MapWarehouseEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/warehouses")
            .WithTags("Warehouses")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetWarehouseById")
            .WithDescription("Get a single warehouse by ID including bank details, holidays and robots")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", QueryAsync)
            .WithName("QueryWarehouses")
            .WithDescription("Search and filter warehouses with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateWarehouse")
            .WithDescription("Update a single warehouse")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        ReadOnlyWarehouseDbContext db,
        ILogger<WarehouseDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await db.Warehouses
                .Include(w => w.BankDetails)
                .Include(w => w.Holidays)
                .Include(w => w.Robots)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            return warehouse is null
                ? Results.NotFound()
                : Results.Ok(warehouse.ToDetailDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving warehouse {WarehouseId}", id);
            return Results.Problem("An error occurred while retrieving the warehouse.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> QueryAsync(
        [AsParameters] WarehouseQueryParameters query,
        ReadOnlyWarehouseDbContext db,
        ILogger<WarehouseDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var queryable = db.Warehouses
                .ApplyFilters(query)
                .ApplySorting(query);

            var totalCount = await queryable.CountAsync(cancellationToken);
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(w => w.ToDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(new PagedResponse<WarehouseDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error querying warehouses");
            return Results.Problem("An error occurred while querying warehouses.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateWarehouseRequest request,
        WarehouseDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        ILogger<WarehouseDbContext> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating warehouse {WarehouseId}", id);

        try
        {
            var warehouse = await db.Warehouses.FindAsync([id], cancellationToken);
            if (warehouse is null) return Results.NotFound();

            request.ApplyTo(warehouse);

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("warehouses", cancellationToken);

            await eventPublisher.PublishAsync(new WarehouseUpdatedEvent
            {
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name
            }, cancellationToken);

            logger.LogInformation("Warehouse {WarehouseId} updated successfully", id);
            return Results.Ok(warehouse.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating warehouse {WarehouseId}", id);
            return Results.Problem("An error occurred while updating the warehouse.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
