namespace BestMed.Common.Messaging;

/// <summary>
/// Publishes integration events to the Service Bus.
/// Inject this into services that need to notify other services of domain changes.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an integration event to its topic on the Service Bus.
    /// The topic name is derived from the event type name (e.g. RoleUpdatedEvent → role-updated).
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
