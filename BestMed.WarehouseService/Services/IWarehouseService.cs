using BestMed.WarehouseService.DTOs;

namespace BestMed.WarehouseService.Services;

/// <summary>
/// Business logic for warehouse operations.
/// Extracted from the endpoint layer so handlers can be unit-tested by mocking this interface.
/// </summary>
public interface IWarehouseService
{
    // ── Read ──────────────────────────────────────────────────────────────────
    Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult> QueryAsync(WarehouseQueryParameters query, CancellationToken cancellationToken);
    Task<IResult> GetNamesAsync(CancellationToken cancellationToken);

    // ── Warehouse write ───────────────────────────────────────────────────────
    Task<IResult> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(Guid id, UpdateWarehouseRequest request, CancellationToken cancellationToken);
    Task<IResult> UpdateConfigAsync(Guid id, UpdateWarehouseConfigRequest request, CancellationToken cancellationToken);
    Task<IResult> UpdateAttachmentAsync(Guid id, Guid docId, CancellationToken cancellationToken);

    // ── Bank details ──────────────────────────────────────────────────────────
    Task<IResult> GetBankDetailAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult> SaveBankDetailAsync(Guid id, SaveWarehouseBankDetailRequest request, CancellationToken cancellationToken);

    // ── Holidays ──────────────────────────────────────────────────────────────
    Task<IResult> GetHolidaysAsync(Guid id, WarehouseHolidayQueryParameters query, CancellationToken cancellationToken);
    Task<IResult> SaveHolidayAsync(Guid id, SaveWarehouseHolidayRequest request, CancellationToken cancellationToken);
    Task<IResult> DeleteHolidayAsync(Guid holidayId, CancellationToken cancellationToken);

    // ── PharmacyToInsert utilities ────────────────────────────────────────────
    Task<IResult> GetPharmacyToInsertAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult> CheckPharmacyToInsertGlobalDrugAsync(Guid id, CancellationToken cancellationToken);
}
