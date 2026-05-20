using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[ReportMedicationSummarySection] table.
/// FK → ReportMedicationSummary.Id, Section.Id. No audit cols.
/// </summary>
[Table("ReportMedicationSummarySection")]
public class ReportMedicationSummarySection : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid ReportMedicationSummaryId { get; set; }

    public Guid SectionId { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(ReportMedicationSummaryId))]
    public ReportMedicationSummary? ReportMedicationSummary { get; set; }

    [ForeignKey(nameof(SectionId))]
    public Section? Section { get; set; }
}
