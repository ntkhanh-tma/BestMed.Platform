using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[FacilityBulkPackGenerateRange] table — pack generation schedule per Facility.
/// No audit columns.
/// </summary>
[Table("FacilityBulkPackGenerateRange")]
public class FacilityBulkPackGenerateRange : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    public Guid? PharmacyId { get; set; }

    public DateTime PackStartDate { get; set; }

    public DateTime PackEndDate { get; set; }

    [StringLength(10)]
    public string? FacilityDay1 { get; set; }

    public int? PackDayOffset { get; set; }

    public DateTime RequestGeneratedDate { get; set; }

    public bool IsMissed { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }
}
