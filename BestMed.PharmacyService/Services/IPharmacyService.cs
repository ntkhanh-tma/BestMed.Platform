using BestMed.PharmacyService.DTOs;

namespace BestMed.PharmacyService.Services;

/// <summary>
/// Business logic for pharmacy operations.
/// Extracted from the endpoint layer so handlers can be unit-tested by mocking this interface.
/// </summary>
public interface IPharmacyService
{
    Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult> QueryAsync(PharmacyQueryParameters query, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(Guid id, UpdatePharmacyRequest request, CancellationToken cancellationToken);
}
