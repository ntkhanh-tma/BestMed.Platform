namespace BestMed.WarehouseService.DTOs;

/// <summary>
/// Lightweight warehouse name item used for dropdown lists.
/// Corresponds to GetAllWareHouseNames / GetWarehouseDetails_DropDown in the legacy controller.
/// </summary>
public sealed record WarehouseNameDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
}
