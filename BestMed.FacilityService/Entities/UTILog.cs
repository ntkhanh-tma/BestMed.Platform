using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[UTILog] table. FK → Facility.Id. No audit cols.
/// </summary>
[Table("UTILog")]
public class UTILog : IEntity
{
    [Key]
    public Guid Id { get; set; }

    [Column("FacilityID")]
    public Guid FacilityId { get; set; }

    [Required]
    [StringLength(100)]
    public string FacilityName { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string AlertType { get; set; } = null!;

    public DateTime AlertDateTime { get; set; }

    [Required]
    [StringLength(40)]
    public string PrescriberNo { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string PrescriberLastName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string PrescriberFirstName { get; set; } = null!;

    [Required]
    [StringLength(15)]
    public string ClinicalDecision { get; set; } = null!;

    public int? ResidentAgeInYears { get; set; }

    [Required]
    [StringLength(20)]
    public string ResidentGender { get; set; } = null!;

    [StringLength(100)]
    public string? Indication { get; set; }

    [StringLength(50)]
    public string? OrganismCultureSensitivity { get; set; }

    [Required]
    [StringLength(200)]
    public string RecommendedDuration { get; set; } = null!;

    [StringLength(200)]
    public string? RecommendedDose { get; set; }

    [Required]
    [StringLength(100)]
    public string RecommendedAgent { get; set; } = null!;

    [StringLength(150)]
    public string? ReasonForNotChangingThePrescription { get; set; }

    [StringLength(250)]
    public string? PrescribedMedication { get; set; }

    [StringLength(100)]
    public string? PrescribedDuration { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }
}
