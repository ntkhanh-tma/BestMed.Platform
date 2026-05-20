namespace BestMed.Common.Messaging.Events;

/// <summary>
/// Published by FacilityService when a facility's data changes.
/// Consumers should invalidate any cached facility data.
/// Communication pattern: Service Bus (async) — multiple services may react.
/// </summary>
public sealed record FacilityUpdatedEvent : IntegrationEvent
{
    public required Guid FacilityId { get; init; }
    public string? FacilityName { get; init; }
}
