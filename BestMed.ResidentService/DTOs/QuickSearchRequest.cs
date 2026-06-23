namespace BestMed.ResidentService.DTOs;

/// <summary>Request body for POST /residents/quick-search.</summary>
public sealed record QuickSearchRequest
{
    public string? SearchTerms { get; init; }
    public Guid? PharmacyId { get; init; }
    public Guid? WarehouseId { get; init; }
}
