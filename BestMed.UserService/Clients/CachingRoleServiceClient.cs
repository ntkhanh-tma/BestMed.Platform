using BestMed.Common.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace BestMed.UserService.Clients;

/// <summary>
/// Caching decorator for <see cref="IRoleServiceClient"/>.
/// Role data changes infrequently; cache for 5 minutes and invalidate via
/// <see cref="InvalidateAsync"/> when a RoleUpdatedEvent arrives from the Service Bus.
/// </summary>
internal sealed class CachingRoleServiceClient(
    IRoleServiceClient inner,
    IMemoryCache cache) : IRoleServiceClient
{
    private const string AllRolesCacheKey = "roles:all";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<IReadOnlyList<RoleContract>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync(AllRolesCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await inner.GetAllRolesAsync(cancellationToken);
        }) ?? [];
    }

    public async Task<RoleContract?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"roles:{roleId}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await inner.GetRoleByIdAsync(roleId, cancellationToken);
        });
    }

    /// <summary>
    /// Evicts all cached role entries. Called when a RoleUpdatedEvent is received.
    /// </summary>
    public static void InvalidateAll(IMemoryCache cache)
    {
        cache.Remove(AllRolesCacheKey);
    }

    public static void InvalidateById(IMemoryCache cache, Guid roleId)
    {
        cache.Remove($"roles:{roleId}");
        cache.Remove(AllRolesCacheKey);
    }
}
