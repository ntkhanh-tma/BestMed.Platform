using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[BatchReferenceFileBuilder] table.
/// FK→ Pharmacy.Id, Document.Id (external).
/// No ClusteredKey. Has CreatedDate/CreatedBy only.
/// </summary>
[Table("BatchReferenceFileBuilder")]
public class BatchReferenceFileBuilder : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PharmacyId { get; set; }

    public Guid DocumentId { get; set; }

    [Required]
    [StringLength(20)]
    public string PackRequestNumber { get; set; } = null!;

    public int? MedicationCount { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid CreatedBy { get; set; }

    // Navigation
    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy Pharmacy { get; set; } = null!;
}
