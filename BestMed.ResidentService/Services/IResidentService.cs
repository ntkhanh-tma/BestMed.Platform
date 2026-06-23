using BestMed.ResidentService.DTOs;

namespace BestMed.ResidentService.Services;

/// <summary>
/// Core resident business operations — owns everything that lived in
/// the legacy IResidentBusiness interface.
/// </summary>
public interface IResidentService
{
    // ── List / Search ──────────────────────────────────────────────────────────
    Task<IResult> GetResidentsAsync(ResidentQueryParameters query, CancellationToken cancellationToken);
    Task<IResult> QuickSearchAsync(QuickSearchRequest request, CancellationToken cancellationToken);
    Task<IResult> GetResidentListAsync(Guid? pharmacyId, Guid? facilityId, Guid? sectionId, Guid? prescriberId, CancellationToken cancellationToken);
    Task<IResult> GetResidentDetailsAsync(Guid residentId, CancellationToken cancellationToken);
    Task<IResult> GetFacilityNameAsync(Guid residentId, CancellationToken cancellationToken);
    Task<IResult> GetMedicationTrackingResidentsAsync(IReadOnlyList<Guid>? facilityIds, CancellationToken cancellationToken);

    // ── Profile ────────────────────────────────────────────────────────────────
    Task<IResult> GetProfileAsync(Guid residentId, Guid? drugId, string defaultTab, CancellationToken cancellationToken);
    Task<IResult> GetHeaderAsync(Guid residentId, bool allergies, bool checkIhi, CancellationToken cancellationToken);
    Task<IResult> GetLastPackedUntilDateAsync(Guid residentId, CancellationToken cancellationToken);
    Task<IResult> GetProfileLockExpiryAsync(Guid residentId, CancellationToken cancellationToken);
    Task<IResult> HasForcedDeleteProfileAsync(Guid residentId, CancellationToken cancellationToken);
    Task<IResult> CheckResidentIsCurrentAsync(Guid residentId, CancellationToken cancellationToken);

    // ── Med Profiles ───────────────────────────────────────────────────────────
    Task<IResult> GetMedProfilesAsync(Guid residentId, bool isActive, int page, int pageSize, CancellationToken cancellationToken);
    Task<IResult> CheckMedProfileCanBeRemovedAsync(Guid profileId, CancellationToken cancellationToken);
    Task<IResult> RemoveMedProfileAsync(Guid profileId, CancellationToken cancellationToken);
    Task<IResult> RemovePendingChartProfileAsync(RemovePendingChartProfileRequest request, CancellationToken cancellationToken);
    Task<IResult> IsProfileEditableAsync(Guid profileId, string status, string lastUpdated, CancellationToken cancellationToken);
    Task<IResult> CheckMedProfileLockAsync(Guid profileId, string lastUpdated, CancellationToken cancellationToken);
    Task<IResult> GetInvalidPreferredBrandPbsAsync(Guid profileId, CancellationToken cancellationToken);
    Task<IResult> CompleteMedChangeVerificationAsync(Guid medProfileId, CancellationToken cancellationToken);

    // ── Pack & Scheduling ──────────────────────────────────────────────────────
    Task<IResult> GetPackLayoutAsync(Guid residentId, Guid? medProfileId, CancellationToken cancellationToken);
    Task<IResult> SaveGroupPackAsync(Dictionary<string, string> groupPacks, CancellationToken cancellationToken);
    Task<IResult> CheckCleanProfileAsync(Guid residentId, DateTime dateTime, CancellationToken cancellationToken);
    Task<IResult> CleanProfileAsync(Guid residentId, CleanProfileRequest request, CancellationToken cancellationToken);
    Task<IResult> GenerateDoseSigningAsync(Guid residentId, GenerateDoseSigningRequest request, CancellationToken cancellationToken);
    Task<IResult> UpdateRegSchedulingAsync(Guid userId, UpdateRegSchedulingRequest request, CancellationToken cancellationToken);

    // ── Prescribers ────────────────────────────────────────────────────────────
    Task<IResult> GetPrescribersDropdownAsync(Guid residentId, int type, CancellationToken cancellationToken);

    // ── VMC flags (ownership stays in Resident) ────────────────────────────────
    Task<IResult> UpdateVmcRequireTransferAsync(Guid residentId, bool setBackToNull, CancellationToken cancellationToken);
    Task<IResult> CheckVmcRequireTransferAndActiveProfileAsync(Guid residentId, CancellationToken cancellationToken);

    // ── Staging & Linking ──────────────────────────────────────────────────────
    Task<IResult> GetResidentStagingMatchedAsync(Guid residentId, CancellationToken cancellationToken);
    Task<IResult> GetMatchedResidentsAsync(Guid residentId, CancellationToken cancellationToken);
    Task<IResult> LinkResidentAsync(Guid residentId, LinkResidentRequest request, CancellationToken cancellationToken);

    // ── Facility offline mode ──────────────────────────────────────────────────
    Task<IResult> IsFacilityOfflineModeAsync(Guid residentId, CancellationToken cancellationToken);
}
