using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[FacilityDoseConfig] table — dose time configuration per Facility.
/// </summary>
[Table("FacilityDoseConfig")]
public class FacilityDoseConfig : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    public TimeOnly? DoseTime { get; set; }

    public bool? SpecialRound { get; set; }

    public int? StartOfRound { get; set; }

    public int? EndOfRound { get; set; }

    public int? DoseOrder { get; set; }

    [StringLength(30)]
    public string? DoseTimeLabel { get; set; }

    [StringLength(30)]
    public string? SecondaryDoseLabel { get; set; }

    [StringLength(30)]
    public string? MealColour { get; set; }

    public long ClusteredKey { get; set; }

    // Audit
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public Guid? LastUpdatedBy { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }
}
