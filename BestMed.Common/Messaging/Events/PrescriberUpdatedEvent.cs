namespace BestMed.Common.Messaging.Events;

/// <summary>
/// Published by PrescriberService when a prescriber's data changes.
/// Consumers (e.g. UserService) should invalidate any cached prescriber data.
/// Communication pattern: Service Bus (async) — multiple services may react.
/// </summary>
public sealed record PrescriberUpdatedEvent : IntegrationEvent
{
    public required Guid PrescriberId { get; init; }
    public string? PrescriberName { get; init; }
    public string? PrescriberCode { get; init; }
}
