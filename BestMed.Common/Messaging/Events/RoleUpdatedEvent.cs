namespace BestMed.Common.Messaging.Events;

/// <summary>
/// Published by RoleService when a role's data changes.
/// Consumers (e.g. UserService) should invalidate any cached role data.
/// Communication pattern: Service Bus (async) — multiple services may react.
/// </summary>
public sealed record RoleUpdatedEvent : IntegrationEvent
{
    public required Guid RoleId { get; init; }
    public string? RoleName { get; init; }
    public string? NormalizedRole { get; init; }
}
