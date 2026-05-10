namespace BestMed.WarehouseService.DTOs;

public sealed record WarehouseBankDetailDto
{
    public Guid Id { get; init; }
    public string BankName { get; init; } = null!;
    public string BSB { get; init; } = null!;
    public string AccountNumber { get; init; } = null!;
}
