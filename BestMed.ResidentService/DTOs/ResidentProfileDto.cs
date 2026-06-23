namespace BestMed.ResidentService.DTOs;

/// <summary>
/// Response for GET /residents/{id}/profile.
/// Carries computed flags in addition to the raw profile fields.
/// </summary>
public sealed record ResidentProfileDto
{
    public Guid Id { get; init; }
    public string? Status { get; init; }
    public bool IsOtherSupplyPharmacy { get; init; }
    public bool IsRestrictedByFacilityConfig { get; init; }
    public Guid FacilityId { get; init; }

    // Computed flags resolved at query time
    public bool HasEditRights { get; init; }
    public bool IsNationalPlatformUser { get; init; }
    public bool ShouldShowWarfarinPopup { get; init; }
    public Guid? WarfarinDrugId { get; init; }
    public bool IsEnableMedicationTracking { get; init; }

    /// <summary>Selected tab to restore on the profile page (default "1").</summary>
    public string DefaultTab { get; init; } = "1";
}
