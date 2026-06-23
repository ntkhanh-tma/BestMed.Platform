namespace BestMed.ResidentService.Services;

/// <summary>
/// HTTP client contract for calling FacilityService (exists at https+http://facility-service).
/// </summary>
public interface IFacilityClient
{
    /// <summary>
    /// Returns true if the facility that contains the given resident is in offline mode.
    /// Calls GET /facilities/{facilityId}/offline-mode on FacilityService.
    /// </summary>
    Task<bool> IsFacilityOfflineModeAsync(Guid facilityId, CancellationToken cancellationToken);
}
