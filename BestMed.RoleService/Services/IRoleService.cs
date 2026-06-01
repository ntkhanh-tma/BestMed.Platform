using BestMed.RoleService.DTOs;

namespace BestMed.RoleService.Services;

/// <summary>
/// Business logic for role operations.
/// Extracted from the endpoint layer so handlers can be unit-tested by mocking this interface.
/// </summary>
public interface IRoleService
{
    Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IResult> QueryAsync(RoleQueryParameters query, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken);
}
