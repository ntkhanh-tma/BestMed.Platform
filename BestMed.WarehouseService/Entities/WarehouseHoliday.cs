using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.WarehouseService.Entities;

/// <summary>
/// Represents the [dbo].[WarehouseHoliday] table. Database-first — do not modify manually.
/// </summary>
[Table("WarehouseHoliday")]
public partial class WarehouseHoliday : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid WarehouseId { get; set; }

    public DateTime HolidayDate { get; set; }

    [StringLength(500)]
    public string? HolidayName { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(10)]
    public string State { get; set; } = null!;

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ClusteredKey { get; set; }

    [ForeignKey(nameof(WarehouseId))]
    public virtual Warehouse Warehouse { get; set; } = null!;
}
