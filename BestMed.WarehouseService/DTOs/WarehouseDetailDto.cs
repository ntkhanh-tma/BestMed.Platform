namespace BestMed.WarehouseService.DTOs;

/// <summary>
/// Full detail DTO for a warehouse — includes child collections.
/// Used in single-entity GET responses.
/// </summary>
public sealed record WarehouseDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? Suburb { get; init; }
    public string? State { get; init; }
    public string? PostCode { get; init; }
    public string? Country { get; init; }
    public string? ContactName { get; init; }
    public string? Phone { get; init; }
    public string? Fax { get; init; }
    public string? Email { get; init; }
    public string? IPAddress { get; init; }
    public string? IPDescription { get; init; }
    public string? ABN { get; init; }
    public string? XMLUserName { get; init; }
    public Guid? StateTimeZoneId { get; init; }
    public Guid? CheckingMachineType { get; init; }
    public Guid? NewUserAttachmentId { get; init; }
    public string? GeoLocations { get; init; }
    public double? GeoRadius { get; init; }
    public string? SachetRobotTypeId { get; init; }
    public string? BlisterRobotTypeId { get; init; }
    public string? YuyamaModelId { get; init; }
    public bool IsMultiSite { get; init; }
    public bool RestrictPreferredBrand { get; init; }
    public bool? HasThirdPartyPacking { get; init; }
    public bool? PharmacyToInsert { get; init; }
    public bool? EnablePasswordAging { get; init; }
    public int? PasswordAging { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? CreatedDate { get; init; }
    public Guid? LastUpdatedBy { get; init; }
    public DateTime? LastUpdatedDate { get; init; }

    public IReadOnlyList<WarehouseBankDetailDto> BankDetails { get; init; } = [];
    public IReadOnlyList<WarehouseHolidayDto> Holidays { get; init; } = [];
    public IReadOnlyList<WarehouseRobotDto> Robots { get; init; } = [];
}
