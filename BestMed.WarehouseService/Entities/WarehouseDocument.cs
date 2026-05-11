using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.WarehouseService.Entities;

/// <summary>
/// Represents the [dbo].[WarehouseDocument] table. Database-first — do not modify manually.
/// </summary>
[Table("WarehouseDocument")]
public partial class WarehouseDocument : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid WarehouseId { get; set; }

    [Required]
    [StringLength(50)]
    public string DocType { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    public byte[] DocContent { get; set; } = null!;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ClusteredKey { get; set; }

    [ForeignKey(nameof(WarehouseId))]
    public virtual Warehouse Warehouse { get; set; } = null!;
}
