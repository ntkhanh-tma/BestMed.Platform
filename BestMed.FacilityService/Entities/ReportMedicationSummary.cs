using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[ReportMedicationSummary] table. FK → Facility.Id.
/// </summary>
[Table("ReportMedicationSummary")]
public class ReportMedicationSummary : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid? FacilityId { get; set; }

    [StringLength(100)]
    public string? FacilityName { get; set; }

    public DateTime? ReportDate { get; set; }

    [StringLength(50)]
    public string? Period { get; set; }

    public int? ReportCategory { get; set; }

    [StringLength(100)]
    public string? MedicationCategory { get; set; }

    public int? NumberOfResident { get; set; }

    public int? TotalNumberOfResidents { get; set; }

    public string? Area { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }

    public ICollection<ReportMedicationSummarySection> ReportMedicationSummarySections { get; set; } = [];
}
