using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[Section] table — child of Facility.
/// </summary>
[Table("Section")]
public class Section : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid? FacilityId { get; set; }

    public string? Name { get; set; }

    [StringLength(20)]
    public string? FredCode { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(20)]
    public string? Fax { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

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
