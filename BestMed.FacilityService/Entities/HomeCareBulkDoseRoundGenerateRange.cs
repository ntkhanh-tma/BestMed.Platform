using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[HomeCareBulkDoseRoundGenerateRange] table. FK → Facility.Id. No audit cols.
/// </summary>
[Table("HomeCareBulkDoseRoundGenerateRange")]
public class HomeCareBulkDoseRoundGenerateRange : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    public Guid PharmacyId { get; set; }

    public DateTime DoseRoundStartDate { get; set; }

    public DateTime DoseRoundEndDate { get; set; }

    [Required]
    [StringLength(5)]
    public string FacilityDay1 { get; set; } = null!;

    public int DoseRoundDayOffset { get; set; }

    public DateTime RequestGeneratedDate { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }
}
