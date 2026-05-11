namespace BestMed.Common.Messaging.Events;

/// <summary>
/// Published by UserService when a user's active status changes.
/// Communication pattern: Service Bus (async) — other services may need to react
/// (e.g. revoke sessions, audit, disable linked records).
/// </summary>
public sealed record UserStatusChangedEvent : IntegrationEvent
{
    public required Guid UserId { get; init; }
    public bool IsActive { get; init; }
    public string? Status { get; init; }
}
