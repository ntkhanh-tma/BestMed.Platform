namespace BestMed.PharmacyService.DTOs;

/// <summary>
/// Response DTO for a pharmacy.
/// </summary>
public sealed record PharmacyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public Guid? WarehouseId { get; init; }
    public bool Active { get; init; }
    public int? PharmacyType { get; init; }
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
    public string? OutOfHours { get; init; }
    public string? ABN { get; init; }
    public string? PharmacyApprovalNumber { get; init; }
    public string? HPIONumber { get; init; }
    public bool? HasPackingFacility { get; init; }
    public bool IsMultiSite { get; init; }
    public bool EnableDashboard { get; init; }
    public string? Tier { get; init; }
    public DateOnly? ProgrammeJoinedDate { get; init; }
}
