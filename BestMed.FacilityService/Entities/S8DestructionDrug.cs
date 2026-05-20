using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[S8DestructionDrug] table.
/// FK → Facility.Id, Resident.Id, Section.Id, User.Id (DestroyedBy).
/// </summary>
[Table("S8DestructionDrug")]
public class S8DestructionDrug : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    public Guid? PharmacyId { get; set; }

    public Guid? ResidentId { get; set; }

    [StringLength(120)]
    public string? ResidentName { get; set; }

    public Guid? SectionId { get; set; }

    public Guid? DrugId { get; set; }

    public string? MedicineName { get; set; }

    [StringLength(20)]
    public string? Strength { get; set; }

    public decimal? Quantity { get; set; }

    public string? Note { get; set; }

    public bool Destroyed { get; set; }

    /// <summary>FK → User (lives in UserService).</summary>
    public Guid? DestroyedBy { get; set; }

    public DateTime? DestroyedDate { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }

    [ForeignKey(nameof(SectionId))]
    public Section? Section { get; set; }
}
