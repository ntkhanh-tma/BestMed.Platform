using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[PackResidentMed] table.
/// FK→ PackResidentRoll.Id, Medicine.Id (external).
/// </summary>
[Table("PackResidentMed")]
public class PackResidentMed : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PackResidentRollId { get; set; }

    public Guid MedicineId { get; set; }

    public Guid DrugId { get; set; }

    [Required]
    [StringLength(20)]
    public string DrugType { get; set; } = null!;

    public int? PRNDoses { get; set; }

    public decimal? BillPackQtyWhole { get; set; }

    public int? BillPackQtyFractional { get; set; }

    public decimal? DoseQty { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(PackResidentRollId))]
    public PackResidentRoll PackResidentRoll { get; set; } = null!;
}
