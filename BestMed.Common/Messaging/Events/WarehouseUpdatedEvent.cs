namespace BestMed.Common.Messaging.Events;

/// <summary>
/// Published by WarehouseService when a warehouse's data changes.
/// Consumers (e.g. services that cache warehouse data) should invalidate
/// their local cache entries for the affected warehouse.
/// </summary>
public sealed record WarehouseUpdatedEvent : IntegrationEvent
{
    public required Guid WarehouseId { get; init; }
    public required string? WarehouseName { get; init; }
}
