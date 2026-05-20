namespace BestMed.PharmacyService.DTOs;

/// <summary>
/// Full detail DTO for a pharmacy — includes child collections and additional fields.
/// Used in single-entity GET responses.
/// </summary>
public sealed record PharmacyDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public Guid? WarehouseId { get; init; }
    public Guid? StateTimeZoneId { get; init; }
    public bool Active { get; init; }
    public int? PharmacyType { get; init; }
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? Suburb { get; init; }
    public string? State { get; init; }
    public string? PostCode { get; init; }
    public string? Country { get; init; }
    public string? ShippingAddress1 { get; init; }
    public string? ShippingAddress2 { get; init; }
    public string? ShippingState { get; init; }
    public string? ShippingSuburb { get; init; }
    public string? ShippingPostCode { get; init; }
    public string? ContactName { get; init; }
    public string? Phone { get; init; }
    public string? Fax { get; init; }
    public string? Email { get; init; }
    public string? OutOfHours { get; init; }
    public string? IPAddress { get; init; }
    public string? IPDescription { get; init; }
    public string? ABN { get; init; }
    public string? PharmacyApprovalNumber { get; init; }
    public string? HPIONumber { get; init; }
    public string? HPIOStatus { get; init; }
    public bool? HasPackingFacility { get; init; }
    public bool? S8DrugPackingAllowed { get; init; }
    public bool IsMultiSite { get; init; }
    public bool? EnablePasswordAging { get; init; }
    public int? PasswordAging { get; init; }
    public string? GeoLocations { get; init; }
    public double? GeoRadius { get; init; }
    public bool EnableDashboard { get; init; }
    public string? Tier { get; init; }
    public DateOnly? ProgrammeJoinedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? CreatedDate { get; init; }
    public Guid? LastUpdatedBy { get; init; }
    public DateTime? LastUpdatedDate { get; init; }

    public IReadOnlyList<PharmacyFacilityDto> Facilities { get; init; } = [];
}

/// <summary>
/// Minimal DTO for facilities linked to a pharmacy.
/// The canonical Facility detail lives in FacilityService.
/// </summary>
public sealed record PharmacyFacilityDto
{
    public Guid Id { get; init; }
}
