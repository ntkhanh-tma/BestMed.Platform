using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[QuarterlyPharmacyGroup] table.
/// FK→ Pharmacy.Id, QuarterlyGroup.Id (external). No audit columns.
/// </summary>
[Table("QuarterlyPharmacyGroup")]
public class QuarterlyPharmacyGroup : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PharmacyId { get; set; }

    public Guid QuarterlyGroupId { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy Pharmacy { get; set; } = null!;
}
