using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.WarehouseService.Entities;

/// <summary>
/// Represents the [dbo].[WarehouseRobot] table. Database-first — do not modify manually.
/// </summary>
[Table("WarehouseRobot")]
public partial class WarehouseRobot : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid WarehouseId { get; set; }

    [Required]
    [StringLength(10)]
    public string Type { get; set; } = null!;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ClusteredKey { get; set; }

    [ForeignKey(nameof(WarehouseId))]
    public virtual Warehouse Warehouse { get; set; } = null!;
}
