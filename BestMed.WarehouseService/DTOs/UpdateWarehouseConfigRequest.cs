using System.ComponentModel.DataAnnotations;

namespace BestMed.WarehouseService.DTOs;

/// <summary>
/// Request DTO for saving or updating warehouse configuration settings.
/// Corresponds to UpdateWarehouseConfig in the legacy WarehouseBusiness.
/// </summary>
public sealed record UpdateWarehouseConfigRequest
{
    public string? SachetRobotTypeId { get; init; }
    public string? BlisterRobotTypeId { get; init; }
    public string? YuyamaModelId { get; init; }
    public Guid? CheckingMachineTypeId { get; init; }

    [StringLength(100)]
    public string? XMLUserName { get; init; }

    [StringLength(100)]
    public string? XMLUserPassword { get; init; }

    [StringLength(11)]
    public string? ABN { get; init; }

    public bool IsMultiSite { get; init; }
    public bool? HasThirdPartyPacking { get; init; }
    public bool? PharmacyToInsert { get; init; }
}
