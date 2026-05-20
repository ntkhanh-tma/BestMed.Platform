namespace BestMed.FacilityService.DTOs;

/// <summary>
/// Full detail DTO for a facility — includes child collections.
/// Used in single-entity GET responses.
/// </summary>
public sealed record FacilityDetailDto
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
    public string? IPAddress { get; init; }
    public string? IPDescription { get; init; }
    public string? GeoLocations { get; init; }
    public double? GeoRadius { get; init; }
    public string? ABN { get; init; }
    public string? HPIONumber { get; init; }
    public string? HPIOStatus { get; init; }
    public bool? EnablePasswordAging { get; init; }
    public int? PasswordAging { get; init; }
    public bool ActiveDirectoryEnabled { get; init; }
    public Guid? TenantId { get; init; }
    public bool? Profit { get; init; }
    public string? Region { get; init; }
    public string? Guidelines { get; init; }
    public string? SSOOption { get; init; }
    public bool? EnableTrustedSSO { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? CreatedDate { get; init; }
    public Guid? LastUpdatedBy { get; init; }
    public DateTime? LastUpdatedDate { get; init; }

    public IReadOnlyList<SectionDto> Sections { get; init; } = [];
}
