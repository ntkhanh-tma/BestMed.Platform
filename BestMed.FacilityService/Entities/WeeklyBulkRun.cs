using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[WeeklyBulkRun] table. FK → Facility.Id, Section.Id. No audit cols.
/// </summary>
[Table("WeeklyBulkRun")]
public class WeeklyBulkRun : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    public Guid? SectionId { get; set; }

    public Guid? PharmacyId { get; set; }

    public DateTime RunDate { get; set; }

    public DateTime PackStartDate { get; set; }

    public DateTime PackEndDate { get; set; }

    public bool Status { get; set; }

    public int RequestType { get; set; }

    public string? FailReason { get; set; }

    public int? ResidentsProcessed { get; set; }

    public int? ResidentsRejected { get; set; }

    public string? RejectedResidentIds { get; set; }

    public Guid? ProcessId { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }

    [ForeignKey(nameof(SectionId))]
    public Section? Section { get; set; }
}
