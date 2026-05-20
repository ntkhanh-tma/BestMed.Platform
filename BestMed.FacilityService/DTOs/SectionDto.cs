namespace BestMed.FacilityService.DTOs;

/// <summary>
/// Response DTO for a section within a facility.
/// </summary>
public sealed record SectionDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? FredCode { get; init; }
    public string? Phone { get; init; }
    public string? Fax { get; init; }
    public string? Email { get; init; }
    public bool IsActive { get; init; }
}
