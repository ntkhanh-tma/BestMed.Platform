using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.Common.Models;
using BestMed.WarehouseService.Data;
using BestMed.WarehouseService.DTOs;
using BestMed.WarehouseService.Entities;
using BestMed.WarehouseService.Mapping;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.WarehouseService.Services;

/// <summary>
/// Implements all warehouse business/data logic.
/// Dependencies are injected so the class is fully unit-testable in isolation.
/// </summary>
public sealed class WarehouseService(
    WarehouseDbContext db,
    ReadOnlyWarehouseDbContext readDb,
    IOutputCacheStore cache,
    IEventPublisher eventPublisher,
    ILogger<WarehouseService> logger) : IWarehouseService
{
    // ── Read ──────────────────────────────────────────────────────────────────

    public async Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await readDb.Warehouses
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

    public async Task<IResult> QueryAsync(WarehouseQueryParameters query, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = readDb.Warehouses
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

    public async Task<IResult> GetNamesAsync(CancellationToken cancellationToken)
    {
        var names = await readDb.Warehouses
            .OrderBy(w => w.Name)
            .Select(w => w.ToNameDto())
            .ToListAsync(cancellationToken);

        return Results.Ok(names);
    }

    // ── Warehouse write ───────────────────────────────────────────────────────

    public async Task<IResult> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = new Warehouse { Id = Guid.NewGuid() };
            request.ApplyTo(warehouse);

            db.Warehouses.Add(warehouse);
            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("warehouses", cancellationToken);

            logger.LogInformation("Warehouse {WarehouseId} created", warehouse.Id);
            return Results.CreatedAtRoute("GetWarehouseById", new { id = warehouse.Id }, warehouse.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating warehouse");
            return Results.Problem("An error occurred while creating the warehouse.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> UpdateAsync(Guid id, UpdateWarehouseRequest request, CancellationToken cancellationToken)
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

    public async Task<IResult> UpdateConfigAsync(Guid id, UpdateWarehouseConfigRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await db.Warehouses.FindAsync([id], cancellationToken);
            if (warehouse is null) return Results.NotFound();

            request.ApplyTo(warehouse);

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("warehouses", cancellationToken);

            logger.LogInformation("Warehouse {WarehouseId} config updated", id);
            return Results.Ok(warehouse.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating config for warehouse {WarehouseId}", id);
            return Results.Problem("An error occurred while updating warehouse configuration.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> UpdateAttachmentAsync(Guid id, Guid docId, CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await db.Warehouses.FindAsync([id], cancellationToken);
            if (warehouse is null) return Results.NotFound();

            warehouse.NewUserAttachmentId = docId;
            warehouse.LastUpdatedDate = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("warehouses", cancellationToken);

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating attachment for warehouse {WarehouseId}", id);
            return Results.Problem("An error occurred while updating the attachment.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ── Bank details ──────────────────────────────────────────────────────────

    public async Task<IResult> GetBankDetailAsync(Guid id, CancellationToken cancellationToken)
    {
        var bankDetail = await readDb.BankDetails
            .FirstOrDefaultAsync(b => b.WarehouseId == id, cancellationToken);

        return bankDetail is null
            ? Results.NotFound()
            : Results.Ok(bankDetail.ToDto());
    }

    public async Task<IResult> SaveBankDetailAsync(Guid id, SaveWarehouseBankDetailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var warehouseExists = await db.Warehouses.AnyAsync(w => w.Id == id, cancellationToken);
            if (!warehouseExists) return Results.NotFound();

            var bankDetail = await db.BankDetails
                .FirstOrDefaultAsync(b => b.WarehouseId == id, cancellationToken);

            if (bankDetail is null)
            {
                bankDetail = new WarehouseBankDetail { Id = Guid.NewGuid(), WarehouseId = id };
                request.ApplyTo(bankDetail);
                db.BankDetails.Add(bankDetail);
            }
            else
            {
                request.ApplyTo(bankDetail);
            }

            if (request.IsMultiSite.HasValue)
            {
                var warehouse = await db.Warehouses.FindAsync([id], cancellationToken);
                if (warehouse is not null)
                {
                    warehouse.IsMultiSite = request.IsMultiSite.Value;
                    warehouse.LastUpdatedDate = DateTime.UtcNow;
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("warehouses", cancellationToken);

            return Results.Ok(bankDetail.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving bank details for warehouse {WarehouseId}", id);
            return Results.Problem("An error occurred while saving bank details.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ── Holidays ──────────────────────────────────────────────────────────────

    public async Task<IResult> GetHolidaysAsync(Guid id, WarehouseHolidayQueryParameters query, CancellationToken cancellationToken)
    {
        var warehouseExists = await readDb.Warehouses.AnyAsync(w => w.Id == id, cancellationToken);
        if (!warehouseExists) return Results.NotFound();

        var q = readDb.Holidays.Where(h => h.WarehouseId == id);

        if (query.PharmacyId.HasValue && query.HasPackingFacility == true)
            q = q.Where(h => h.PharmacyId == query.PharmacyId.Value);

        var totalCount = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(h => h.HolidayDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(h => h.ToDto())
            .ToListAsync(cancellationToken);

        return Results.Ok(new PagedResponse<WarehouseHolidayDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        });
    }

    public async Task<IResult> SaveHolidayAsync(Guid id, SaveWarehouseHolidayRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await db.Warehouses.FindAsync([id], cancellationToken);
            if (warehouse is null) return Results.NotFound();

            WarehouseHoliday holiday;
            bool isNew;

            if (request.Id.HasValue && request.Id.Value != Guid.Empty)
            {
                holiday = await db.Holidays.FindAsync([request.Id.Value], cancellationToken);
                if (holiday is null) return Results.NotFound();
                isNew = false;
            }
            else
            {
                holiday = new WarehouseHoliday
                {
                    Id = Guid.NewGuid(),
                    WarehouseId = id,
                    PharmacyId = request.PharmacyId,
                    CreatedBy = Guid.Empty,
                    CreatedDate = DateTime.UtcNow
                };
                isNew = true;
            }

            holiday.HolidayDate = request.HolidayDate;
            holiday.HolidayName = request.HolidayName;
            holiday.Description = request.Description;
            holiday.State = warehouse.State ?? string.Empty;
            holiday.UpdatedBy = Guid.Empty;
            holiday.UpdatedDate = DateTime.UtcNow;

            if (isNew)
                db.Holidays.Add(holiday);

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("warehouses", cancellationToken);

            return isNew
                ? Results.Created($"/warehouses/{id}/holidays", holiday.ToDto())
                : Results.Ok(holiday.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving holiday for warehouse {WarehouseId}", id);
            return Results.Problem("An error occurred while saving the holiday.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> DeleteHolidayAsync(Guid holidayId, CancellationToken cancellationToken)
    {
        try
        {
            var holiday = await db.Holidays.FindAsync([holidayId], cancellationToken);
            if (holiday is null) return Results.NotFound();

            db.Holidays.Remove(holiday);
            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("warehouses", cancellationToken);

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting holiday {HolidayId}", holidayId);
            return Results.Problem("An error occurred while deleting the holiday.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    // ── PharmacyToInsert utilities ────────────────────────────────────────────

    public async Task<IResult> GetPharmacyToInsertAsync(Guid id, CancellationToken cancellationToken)
    {
        var warehouse = await readDb.Warehouses
            .AsNoTracking()
            .Select(w => new { w.Id, w.PharmacyToInsert })
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        return warehouse is null
            ? Results.NotFound()
            : Results.Ok(new { PharmacyToInsert = warehouse.PharmacyToInsert ?? false });
    }

    public async Task<IResult> CheckPharmacyToInsertGlobalDrugAsync(Guid id, CancellationToken cancellationToken)
    {
        // Verifies whether the warehouse exists; GlobalDrugWarehouseProperty is owned by
        // the legacy system and not replicated here. We verify warehouse existence only.
        var exists = await readDb.Warehouses.AnyAsync(w => w.Id == id, cancellationToken);
        if (!exists) return Results.NotFound();

        // NOTE: GlobalDrugWarehouseProperty data lives in the legacy BESThealth database.
        // This endpoint returns a 501 until the global drug data is migrated or a
        // cross-service query mechanism is established.
        return Results.Problem(
            "GlobalDrugWarehouseProperty data is not yet available in this service.",
            statusCode: StatusCodes.Status501NotImplemented);
    }
}
