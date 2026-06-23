namespace BestMed.ResidentService.Services;

/// <summary>
/// HTTP client contract for the MedicationTracking domain service.
/// TODO: Implement when the MedicationTracking microservice is built.
/// </summary>
public interface IMedicationTrackingClient
{
    /// <summary>Returns true when medication tracking is enabled for the given resident.</summary>
    Task<bool> IsShowMedicationTrackingForResidentAsync(Guid residentId, CancellationToken cancellationToken);

    /// <summary>Returns the packed medication tracking history for the given resident.</summary>
    Task<IResult> GetPackedHistoryAsync(Guid residentId, CancellationToken cancellationToken);

    /// <summary>Returns the non-packed (script-level) tracking history for the given resident.</summary>
    Task<IResult> GetNonPackedHistoryAsync(Guid residentId, CancellationToken cancellationToken);
}
