using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.UserService.Clients;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BestMed.UserService.Messaging;

/// <summary>
/// Handles RoleUpdatedEvent received from the Service Bus.
/// Invalidates the local role cache so the next read fetches fresh data from RoleService via HTTP.
/// Communication pattern: Service Bus (async) — decoupled, no direct dependency on RoleService at event time.
/// </summary>
internal sealed class RoleUpdatedEventHandler(
    IMemoryCache cache,
    ILogger<RoleUpdatedEventHandler> logger) : IEventHandler<RoleUpdatedEvent>
{
    public Task HandleAsync(RoleUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        CachingRoleServiceClient.InvalidateById(cache, @event.RoleId);
        logger.LogInformation("Role cache invalidated for RoleId={RoleId} after RoleUpdatedEvent", @event.RoleId);
        return Task.CompletedTask;
    }
}
