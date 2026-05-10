namespace BestMed.WarehouseService.DTOs;

public sealed record WarehouseRobotDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = null!;
}
