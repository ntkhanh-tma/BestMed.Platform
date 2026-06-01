using BestMed.FacilityService.DTOs;

namespace BestMed.FacilityService.Services;

/// <summary>
/// Business logic for facility operations.
/// Extracted from the endpoint layer so handlers can be unit-tested by mocking this interface.
/// </summary>
public interface IFacilityService
{
    Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult> QueryAsync(FacilityQueryParameters query, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(Guid id, UpdateFacilityRequest request, CancellationToken cancellationToken);
}
