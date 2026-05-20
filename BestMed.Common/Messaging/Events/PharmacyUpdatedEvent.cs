namespace BestMed.Common.Messaging.Events;

/// <summary>
/// Published by PharmacyService when a pharmacy's data changes.
/// Consumers should invalidate any cached pharmacy data.
/// Communication pattern: Service Bus (async) — multiple services may react.
/// </summary>
public sealed record PharmacyUpdatedEvent : IntegrationEvent
{
    public required Guid PharmacyId { get; init; }
    public string? PharmacyName { get; init; }
}
