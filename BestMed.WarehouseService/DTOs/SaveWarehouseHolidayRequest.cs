using System.ComponentModel.DataAnnotations;

namespace BestMed.WarehouseService.DTOs;

/// <summary>
/// Request DTO for creating or updating a warehouse holiday.
/// Corresponds to SubmitWarehouseHoliday in the legacy WarehouseBusiness.
/// </summary>
public sealed record SaveWarehouseHolidayRequest
{
    /// <summary>When present, updates an existing holiday record.</summary>
    public Guid? Id { get; init; }

    public Guid WarehouseId { get; init; }

    /// <summary>
    /// Optional — when set, the holiday is scoped to a specific pharmacy linked to this warehouse.
    /// PharmacyService is not called here; the PharmacyId is stored as a reference only.
    /// </summary>
    public Guid? PharmacyId { get; init; }

    [Required]
    public DateTime HolidayDate { get; init; }

    [StringLength(500)]
    public string? HolidayName { get; init; }

    [StringLength(2000)]
    public string? Description { get; init; }
}
