using BestMed.UserService.DTOs;

namespace BestMed.UserService.Services;

/// <summary>
/// Business logic for user operations.
/// Extracted from the endpoint layer so handlers can be unit-tested by mocking this interface.
/// </summary>
public interface IUserService
{
    Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken);
    Task<IResult> QueryAsync(UserQueryParameters query, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task<IResult> UpdateStatusAsync(Guid id, UpdateUserStatusRequest request, CancellationToken cancellationToken);
    Task<IResult> BulkUpdateAsync(BulkUpdateUsersRequest request, CancellationToken cancellationToken);
}
