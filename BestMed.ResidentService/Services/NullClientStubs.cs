using BestMed.ResidentService.DTOs;

namespace BestMed.ResidentService.Services;

/// <summary>
/// Null-object stub for IDrugClient.
/// Returns 501 until the Drug microservice is built and registered.
/// </summary>
internal sealed class NullDrugClient : IDrugClient
{
    public Task<bool> IsMainVariableDrugAsync(Guid drugId, CancellationToken cancellationToken)
        => Task.FromResult(false); // Safe default: don't show Warfarin popup

    public Task<string?> GetDrugNameAsync(Guid drugId, CancellationToken cancellationToken)
        => Task.FromResult<string?>(null);
}

/// <summary>
/// Null-object stub for IMedicationTrackingClient.
/// Returns 501 until the MedicationTracking microservice is built and registered.
/// </summary>
internal sealed class NullMedicationTrackingClient : IMedicationTrackingClient
{
    public Task<bool> IsShowMedicationTrackingForResidentAsync(Guid residentId, CancellationToken cancellationToken)
        => Task.FromResult(false);

    public Task<IResult> GetPackedHistoryAsync(Guid residentId, CancellationToken cancellationToken)
        => Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));

    public Task<IResult> GetNonPackedHistoryAsync(Guid residentId, CancellationToken cancellationToken)
        => Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
}

/// <summary>
/// Null-object stub for IObservationsClient.
/// Returns 501 until the OBS microservice is built and registered.
/// </summary>
internal sealed class NullObservationsClient : IObservationsClient
{
    public Task<IResult> GetObsTreeAsync(Guid residentId, CancellationToken cancellationToken)
        => Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));

    public Task<IResult> GetObsHistoryAsync(Guid residentId, string obsType, CancellationToken cancellationToken)
        => Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
}

/// <summary>
/// Null-object stub for IOrderingClient.
/// Returns 501 until the Ordering microservice is built and registered.
/// </summary>
internal sealed class NullOrderingClient : IOrderingClient
{
    public Task<IResult> CreateResidentOrderAsync(Guid residentId, CreateResidentOrderRequest request, CancellationToken cancellationToken)
        => Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
}
