namespace BestMed.Common.Messaging;

/// <summary>
/// Base record for all integration events.
/// Provides <see cref="EventId"/> and <see cref="OccurredAt"/> automatically.
/// </summary>
public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
