namespace BestMed.Common.Contracts;

/// <summary>
/// Shared contract DTO for prescriber data returned by PrescriberService.
/// Used by consumers (e.g. UserService) without a direct project reference to PrescriberService.
/// </summary>
public sealed record PrescriberContract
{
    public Guid Id { get; init; }
    public string PrescriberName { get; init; } = null!;
    public string PrescriberCode { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? AHPRANumber { get; init; }
    public string? HPIINumber { get; init; }
    public bool IsActive { get; init; }
}
