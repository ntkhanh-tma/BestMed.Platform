namespace BestMed.WarehouseService.DTOs;

/// <summary>
/// Response DTO for a warehouse (summary — used in list/query results).
/// </summary>
public sealed record WarehouseDto
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
    public string? ABN { get; init; }
    public bool IsMultiSite { get; init; }
    public bool RestrictPreferredBrand { get; init; }
    public bool? HasThirdPartyPacking { get; init; }
    public Guid? StateTimeZoneId { get; init; }
    public DateTime? CreatedDate { get; init; }
    public DateTime? LastUpdatedDate { get; init; }
}
