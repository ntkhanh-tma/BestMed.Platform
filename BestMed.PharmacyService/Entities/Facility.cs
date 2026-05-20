using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[Facility] table as owned by the Pharmacy aggregate.
/// This is a local projection; the canonical Facility definition lives in the Facility domain.
/// </summary>
[Table("Facility")]
public class Facility : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid? PharmacyId { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy? Pharmacy { get; set; }
}
