using BestMed.ResidentService.DTOs;

namespace BestMed.ResidentService.Services;

/// <summary>
/// HTTP client contract for the Ordering domain service.
/// TODO: Implement when the Ordering microservice is built.
/// </summary>
public interface IOrderingClient
{
    /// <summary>Creates a drug order for the given resident.</summary>
    Task<IResult> CreateResidentOrderAsync(Guid residentId, CreateResidentOrderRequest request, CancellationToken cancellationToken);
}
