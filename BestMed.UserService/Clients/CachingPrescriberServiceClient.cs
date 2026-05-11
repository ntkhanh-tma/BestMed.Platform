using BestMed.Common.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace BestMed.UserService.Clients;

/// <summary>
/// Caching decorator for <see cref="IPrescriberServiceClient"/>.
/// Prescriber data changes infrequently; cache for 5 minutes and invalidate via
/// <see cref="InvalidateById"/> when a PrescriberUpdatedEvent arrives from the Service Bus.
/// </summary>
internal sealed class CachingPrescriberServiceClient(
    IPrescriberServiceClient inner,
    IMemoryCache cache) : IPrescriberServiceClient
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<PrescriberContract?> GetPrescriberByIdAsync(Guid prescriberId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"prescribers:{prescriberId}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await inner.GetPrescriberByIdAsync(prescriberId, cancellationToken);
        });
    }

    public static void InvalidateById(IMemoryCache cache, Guid prescriberId)
    {
        cache.Remove($"prescribers:{prescriberId}");
    }
}
