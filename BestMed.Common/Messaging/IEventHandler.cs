namespace BestMed.Common.Messaging;

/// <summary>
/// Handles a specific integration event received from the Service Bus.
/// Implement this interface in each service that needs to react to a domain event.
/// </summary>
public interface IEventHandler<in TEvent> where TEvent : IIntegrationEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
