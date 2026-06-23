namespace BestMed.Common.Messaging.Events;

/// <summary>
/// Published by ResidentService when a resident's core data changes.
/// Consumers should invalidate any cached resident data.
/// Communication pattern: Service Bus (async) — multiple services may react.
/// </summary>
public sealed record ResidentUpdatedEvent : IntegrationEvent
{
    public required Guid ResidentId { get; init; }
    public string? DisplayName { get; init; }
    public Guid? FacilityId { get; init; }
}
