using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[NonBhsUserPharmacy] table — M2M link between User and SupplyPharmacy.
/// </summary>
[Table("NonBhsUserPharmacy")]
public class NonBhsUserPharmacy : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PharmacyId { get; set; }

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
