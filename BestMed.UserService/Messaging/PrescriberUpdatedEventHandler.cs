using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.UserService.Clients;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BestMed.UserService.Messaging;

/// <summary>
/// Handles PrescriberUpdatedEvent received from the Service Bus.
/// Invalidates the local prescriber cache so the next read fetches fresh data.
/// Communication pattern: Service Bus (async) — decoupled from PrescriberService.
/// </summary>
internal sealed class PrescriberUpdatedEventHandler(
    IMemoryCache cache,
    ILogger<PrescriberUpdatedEventHandler> logger) : IEventHandler<PrescriberUpdatedEvent>
{
    public Task HandleAsync(PrescriberUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        CachingPrescriberServiceClient.InvalidateById(cache, @event.PrescriberId);
        logger.LogInformation("Prescriber cache invalidated for PrescriberId={PrescriberId} after PrescriberUpdatedEvent",
            @event.PrescriberId);
        return Task.CompletedTask;
    }
}
