namespace BestMed.ResidentService.Services;

/// <summary>
/// HTTP client contract for the OBS (clinical observations) domain service.
/// TODO: Implement when the OBS microservice is built.
/// </summary>
public interface IObservationsClient
{
    /// <summary>Returns the latest OBS tree charts for the given resident.</summary>
    Task<IResult> GetObsTreeAsync(Guid residentId, CancellationToken cancellationToken);

    /// <summary>Returns observation history filtered by obs type.</summary>
    Task<IResult> GetObsHistoryAsync(Guid residentId, string obsType, CancellationToken cancellationToken);
}
