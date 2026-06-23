namespace BestMed.ResidentService.DTOs;

/// <summary>
/// Response for GET /residents/{id}/header — drives the nav-bar partial.
/// </summary>
public sealed record ResidentHeaderDto
{
    public Guid Id { get; init; }
    public string? DisplayName { get; init; }
    public Guid FacilityId { get; init; }
    public string? Status { get; init; }

    /// <summary>Photo timestamp. Returns "2000-01-01T00:00:00" when null (legacy sentinel).</summary>
    public string PhotoLastUpdate { get; init; } = "2000-01-01T00:00:00";

    public bool EnableDocumentManagement { get; init; }
    public bool IsEnableResidentDocumentManagement { get; init; }
    public bool IsOtherSupplyPharmacy { get; init; }
    public bool HasEditRights { get; init; }
    public bool IsEnableMedicationTracking { get; init; }

    /// <summary>
    /// IHI warning text. Empty string when the calling user lacks edit rights
    /// (never expose the warning to read-only users).
    /// </summary>
    public string ResidentMissedIHIWarning { get; init; } = string.Empty;
}
