namespace BestMed.ResidentService.DTOs;

/// <summary>Lightweight resident item used in list/search results.</summary>
public sealed record ResidentDto
{
    public Guid Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? DisplayName { get; init; }
    public Guid FacilityId { get; init; }
    public Guid? SectionId { get; init; }
    public string? Status { get; init; }
    public string? FredCode { get; init; }
}
