using System.ComponentModel.DataAnnotations;

namespace BestMed.WarehouseService.DTOs;

/// <summary>
/// Request DTO for creating or updating warehouse bank details.
/// </summary>
public sealed record SaveWarehouseBankDetailRequest
{
    [Required]
    [StringLength(50)]
    public string BankName { get; init; } = null!;

    [Required]
    [StringLength(10)]
    public string BSB { get; init; } = null!;

    [Required]
    [StringLength(10)]
    public string AccountNumber { get; init; } = null!;

    public bool? IsMultiSite { get; init; }
}
