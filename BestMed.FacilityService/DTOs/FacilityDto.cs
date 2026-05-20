namespace BestMed.FacilityService.DTOs;

/// <summary>
/// Response DTO for a facility.
/// </summary>
public sealed record FacilityDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public Guid PharmacyId { get; init; }
    public Guid? StateTimeZoneId { get; init; }
    public int Active { get; init; }
    public string? FacilityType { get; init; }
    public string? ContactName { get; init; }
    public string? FredCode { get; init; }
    public string? RacId { get; init; }
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? Suburb { get; init; }
    public string? State { get; init; }
    public string? PostCode { get; init; }
    public string? Country { get; init; }
    public string? Phone { get; init; }
    public string? Fax { get; init; }
    public string? Email { get; init; }
    public string? ABN { get; init; }
    public string? HPIONumber { get; init; }
    public string? Region { get; init; }
    public bool ActiveDirectoryEnabled { get; init; }
    public string? SSOOption { get; init; }
}
