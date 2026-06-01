namespace BestMed.WarehouseService.DTOs;

/// <summary>
/// Query parameters for filtering holiday lists.
/// Corresponds to GetWareHouseHolidayList in the legacy WarehouseBusiness.
/// </summary>
public sealed record WarehouseHolidayQueryParameters
{
    public Guid WarehouseId { get; init; }

    /// <summary>When set, narrows results to holidays for this pharmacy.</summary>
    public Guid? PharmacyId { get; init; }

    /// <summary>When true and PharmacyId is provided, shows pharmacy-scoped holidays.</summary>
    public bool? HasPackingFacility { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
