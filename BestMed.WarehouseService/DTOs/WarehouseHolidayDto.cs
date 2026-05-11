namespace BestMed.WarehouseService.DTOs;

public sealed record WarehouseHolidayDto
{
    public Guid Id { get; init; }
    public DateTime HolidayDate { get; init; }
    public string? HolidayName { get; init; }
    public string? Description { get; init; }
    public string State { get; init; } = null!;
}
