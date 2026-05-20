using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[FacilityDoseFilterConfig] table — dose filter/category config per Facility.
/// Has CreatedDate/CreatedBy only (no LastUpdated).
/// </summary>
[Table("FacilityDoseFilterConfig")]
public class FacilityDoseFilterConfig : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    [StringLength(100)]
    public string? CategoryName { get; set; }

    public int? CategoryOrder { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }
}
