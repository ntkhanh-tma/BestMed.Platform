namespace BestMed.ResidentService.DTOs;

/// <summary>Single item in a quick-search result list.</summary>
public sealed record QuickSearchItemDto
{
    public Guid ResidentId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? DisplayName { get; init; }
    public Guid FacilityId { get; init; }
    public string? FacilityName { get; init; }
    public string? Status { get; init; }
}
