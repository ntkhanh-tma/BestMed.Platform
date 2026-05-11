using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.WarehouseService.Entities;

/// <summary>
/// Represents the [dbo].[WarehouseBankDetail] table. Database-first — do not modify manually.
/// </summary>
[Table("WarehouseBankDetail")]
public partial class WarehouseBankDetail : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid WarehouseId { get; set; }

    [Required]
    [StringLength(50)]
    public string BankName { get; set; } = null!;

    [Required]
    [StringLength(10)]
    public string BSB { get; set; } = null!;

    [Required]
    [StringLength(10)]
    public string AccountNumber { get; set; } = null!;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ClusteredKey { get; set; }

    [ForeignKey(nameof(WarehouseId))]
    public virtual Warehouse Warehouse { get; set; } = null!;
}
