using BestMed.Common.Contracts;

namespace BestMed.UserService.Clients;

/// <summary>
/// Typed HTTP client for querying RoleService.
/// Communication pattern: Direct HTTP (synchronous) — used when UserService needs
/// to validate a RoleId or look up role details as part of a live request.
/// Results are cached by <see cref="CachingRoleServiceClient"/>.
/// </summary>
public interface IRoleServiceClient
{
    /// <summary>Returns all roles. Callers should rely on the caching decorator.</summary>
    Task<IReadOnlyList<RoleContract>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a single role by ID, or null if not found.</summary>
    Task<RoleContract?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
}
