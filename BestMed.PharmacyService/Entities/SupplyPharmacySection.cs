using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[SupplyPharmacySection] table.
/// FK→ SupplyPharmacy.Id (as PharmacyId), Section.Id (external).
/// </summary>
[Table("SupplyPharmacySection")]
public class SupplyPharmacySection : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    public Guid PharmacyId { get; set; }

    public Guid SectionId { get; set; }

    [Required]
    [StringLength(20)]
    public string SectionCode { get; set; } = null!;

    public long ClusteredKey { get; set; }

    // Audit columns
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public Guid? LastUpdatedBy { get; set; }

    // Navigation
    [ForeignKey(nameof(PharmacyId))]
    public SupplyPharmacy SupplyPharmacy { get; set; } = null!;
}
