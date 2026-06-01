using BestMed.WarehouseService.DTOs;
using BestMed.WarehouseService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.WarehouseService.Endpoints;

public static class WarehouseEndpoints
{
    public static IEndpointRouteBuilder MapWarehouseEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/warehouses")
            .WithTags("Warehouses")
            .RequireAuthorization();

        // ── Read ────────────────────────────────────────────────────────────────
        group.MapGet("/{id:guid}", (Guid id, IWarehouseService svc, CancellationToken ct)
                => svc.GetByIdAsync(id, ct))
            .WithName("GetWarehouseById")
            .WithDescription("Get a single warehouse by ID including bank details, holidays and robots")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", ([AsParameters] WarehouseQueryParameters query, IWarehouseService svc, CancellationToken ct)
                => svc.QueryAsync(query, ct))
            .WithName("QueryWarehouses")
            .WithDescription("Search and filter warehouses with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapGet("/names", (IWarehouseService svc, CancellationToken ct)
                => svc.GetNamesAsync(ct))
            .WithName("GetWarehouseNames")
            .WithDescription("Get all warehouse id/name pairs for dropdown lists")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("query");

        // ── Write — warehouse ───────────────────────────────────────────────────
        group.MapPost("/", ([FromBody] CreateWarehouseRequest request, IWarehouseService svc, CancellationToken ct)
                => svc.CreateAsync(request, ct))
            .WithName("CreateWarehouse")
            .WithDescription("Create a new warehouse")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPut("/{id:guid}", (Guid id, [FromBody] UpdateWarehouseRequest request, IWarehouseService svc, CancellationToken ct)
                => svc.UpdateAsync(id, request, ct))
            .WithName("UpdateWarehouse")
            .WithDescription("Update warehouse info fields")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPut("/{id:guid}/config", (Guid id, [FromBody] UpdateWarehouseConfigRequest request, IWarehouseService svc, CancellationToken ct)
                => svc.UpdateConfigAsync(id, request, ct))
            .WithName("UpdateWarehouseConfig")
            .WithDescription("Update warehouse configuration (robots, XML credentials, packing options)")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPut("/{id:guid}/attachment/{docId:guid}", (Guid id, Guid docId, IWarehouseService svc, CancellationToken ct)
                => svc.UpdateAttachmentAsync(id, docId, ct))
            .WithName("UpdateWarehouseAttachment")
            .WithDescription("Associate a new-user attachment document with this warehouse")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // ── Bank details ────────────────────────────────────────────────────────
        group.MapGet("/{id:guid}/bank", (Guid id, IWarehouseService svc, CancellationToken ct)
                => svc.GetBankDetailAsync(id, ct))
            .WithName("GetWarehouseBankDetail")
            .WithDescription("Get bank details for a warehouse")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapPut("/{id:guid}/bank", (Guid id, [FromBody] SaveWarehouseBankDetailRequest request, IWarehouseService svc, CancellationToken ct)
                => svc.SaveBankDetailAsync(id, request, ct))
            .WithName("SaveWarehouseBankDetail")
            .WithDescription("Create or update bank details for a warehouse")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // ── Holidays ────────────────────────────────────────────────────────────
        group.MapGet("/{id:guid}/holidays", (Guid id, [AsParameters] WarehouseHolidayQueryParameters query, IWarehouseService svc, CancellationToken ct)
                => svc.GetHolidaysAsync(id, query, ct))
            .WithName("GetWarehouseHolidays")
            .WithDescription("Get paginated holiday list for a warehouse, optionally scoped to a pharmacy")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPost("/{id:guid}/holidays", (Guid id, [FromBody] SaveWarehouseHolidayRequest request, IWarehouseService svc, CancellationToken ct)
                => svc.SaveHolidayAsync(id, request, ct))
            .WithName("SaveWarehouseHoliday")
            .WithDescription("Create or update a warehouse holiday")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapDelete("/holidays/{holidayId:guid}", (Guid holidayId, IWarehouseService svc, CancellationToken ct)
                => svc.DeleteHolidayAsync(holidayId, ct))
            .WithName("DeleteWarehouseHoliday")
            .WithDescription("Remove a warehouse holiday by ID")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // ── Pharmacy-to-Insert utilities ────────────────────────────────────────
        group.MapGet("/{id:guid}/pharmacy-to-insert", (Guid id, IWarehouseService svc, CancellationToken ct)
                => svc.GetPharmacyToInsertAsync(id, ct))
            .WithName("GetWarehousePharmacyToInsert")
            .WithDescription("Check whether PharmacyToInsert is set for a warehouse")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/{id:guid}/pharmacy-to-insert/global-drug-check", (Guid id, IWarehouseService svc, CancellationToken ct)
                => svc.CheckPharmacyToInsertGlobalDrugAsync(id, ct))
            .WithName("CheckPharmacyToInsertOnGlobalDrug")
            .WithDescription("Check whether any GlobalDrugWarehouseProperty has a PharmacyToInsert > NONE for this warehouse")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        return routes;
    }
}

