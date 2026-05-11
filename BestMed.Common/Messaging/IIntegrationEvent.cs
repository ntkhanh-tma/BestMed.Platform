namespace BestMed.Common.Messaging;

/// <summary>
/// Marker interface for all integration events published to the Service Bus.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>Unique ID for this event occurrence.</summary>
    Guid EventId { get; }

    /// <summary>UTC timestamp when the event was raised.</summary>
    DateTime OccurredAt { get; }
}
