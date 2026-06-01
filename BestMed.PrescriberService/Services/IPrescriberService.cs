using BestMed.PrescriberService.DTOs;

namespace BestMed.PrescriberService.Services;

/// <summary>
/// Business logic for prescriber operations.
/// Extracted from the endpoint layer so handlers can be unit-tested by mocking this interface.
/// </summary>
public interface IPrescriberService
{
    Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult> QueryAsync(PrescriberQueryParameters query, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(Guid id, UpdatePrescriberRequest request, CancellationToken cancellationToken);
}
