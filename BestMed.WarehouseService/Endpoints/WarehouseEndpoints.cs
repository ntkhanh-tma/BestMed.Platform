using BestMed.Common.Constants;
using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.Common.Models;
using BestMed.WarehouseService.Data;
using BestMed.WarehouseService.DTOs;
using BestMed.WarehouseService.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

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
        CancellationToken cancellationToken)
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

    private static async Task<IResult> QueryAsync(
        [AsParameters] WarehouseQueryParameters query,
        ReadOnlyWarehouseDbContext db,
        CancellationToken cancellationToken)
    {
        var queryable = db.Warehouses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
            queryable = queryable.Where(w => w.Name.Contains(query.Name));

        if (!string.IsNullOrWhiteSpace(query.Suburb))
            queryable = queryable.Where(w => w.Suburb != null && w.Suburb.Contains(query.Suburb));

        if (!string.IsNullOrWhiteSpace(query.State))
            queryable = queryable.Where(w => w.State == query.State);

        if (query.IsMultiSite.HasValue)
            queryable = queryable.Where(w => w.IsMultiSite == query.IsMultiSite.Value);

        var asc = SortDirection.IsAscending(query.SortDirection);
        queryable = query.SortBy?.ToLowerInvariant() switch
        {
            "suburb" => asc
                ? queryable.OrderBy(w => w.Suburb)
                : queryable.OrderByDescending(w => w.Suburb),
            "state" => asc
                ? queryable.OrderBy(w => w.State)
                : queryable.OrderByDescending(w => w.State),
            _ => asc
                ? queryable.OrderBy(w => w.Name)
                : queryable.OrderByDescending(w => w.Name)
        };

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

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateWarehouseRequest request,
        WarehouseDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        CancellationToken cancellationToken)
    {
        var warehouse = await db.Warehouses.FindAsync([id], cancellationToken);
        if (warehouse is null) return Results.NotFound();

        if (request.Name is not null) warehouse.Name = request.Name;
        if (request.Address1 is not null) warehouse.Address1 = request.Address1;
        if (request.Address2 is not null) warehouse.Address2 = request.Address2;
        if (request.Suburb is not null) warehouse.Suburb = request.Suburb;
        if (request.State is not null) warehouse.State = request.State;
        if (request.PostCode is not null) warehouse.PostCode = request.PostCode;
        if (request.Country is not null) warehouse.Country = request.Country;
        if (request.ContactName is not null) warehouse.ContactName = request.ContactName;
        if (request.Phone is not null) warehouse.Phone = request.Phone;
        if (request.Fax is not null) warehouse.Fax = request.Fax;
        if (request.Email is not null) warehouse.Email = request.Email;
        if (request.IPDescription is not null) warehouse.IPDescription = request.IPDescription;
        if (request.ABN is not null) warehouse.ABN = request.ABN;
        if (request.StateTimeZoneId.HasValue) warehouse.StateTimeZoneId = request.StateTimeZoneId.Value;
        if (request.IsMultiSite.HasValue) warehouse.IsMultiSite = request.IsMultiSite.Value;
        if (request.RestrictPreferredBrand.HasValue) warehouse.RestrictPreferredBrand = request.RestrictPreferredBrand.Value;
        if (request.HasThirdPartyPacking.HasValue) warehouse.HasThirdPartyPacking = request.HasThirdPartyPacking.Value;
        if (request.PharmacyToInsert.HasValue) warehouse.PharmacyToInsert = request.PharmacyToInsert.Value;
        if (request.EnablePasswordAging.HasValue) warehouse.EnablePasswordAging = request.EnablePasswordAging.Value;
        if (request.PasswordAging.HasValue) warehouse.PasswordAging = request.PasswordAging.Value;
        warehouse.LastUpdatedDate = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        await cache.EvictByTagAsync("warehouses", cancellationToken);

        // Pattern: Service Bus (async) — notify other services that warehouse data has changed.
        await eventPublisher.PublishAsync(new WarehouseUpdatedEvent
        {
            WarehouseId = warehouse.Id,
            WarehouseName = warehouse.Name
        }, cancellationToken);

        return Results.Ok(warehouse.ToDto());
    }
}
