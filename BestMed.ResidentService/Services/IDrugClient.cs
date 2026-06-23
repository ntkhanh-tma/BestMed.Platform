namespace BestMed.ResidentService.Services;

/// <summary>
/// HTTP client contract for the Drug domain service.
/// TODO: Implement when the Drug microservice is built.
///       Register in ServiceRegistration.cs and wire via Aspire service discovery.
/// </summary>
public interface IDrugClient
{
    /// <summary>Returns true when the given drug is a main variable drug (e.g. Warfarin).</summary>
    Task<bool> IsMainVariableDrugAsync(Guid drugId, CancellationToken cancellationToken);

    /// <summary>Returns the drug's display name; null if not found.</summary>
    Task<string?> GetDrugNameAsync(Guid drugId, CancellationToken cancellationToken);
}
