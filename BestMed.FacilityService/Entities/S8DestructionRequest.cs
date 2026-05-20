using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[S8DestructionRequest] table.
/// FK → Facility.Id, User.Id (ProcessedBy). No ClusteredKey. No audit cols.
/// </summary>
[Table("S8DestructionRequest")]
public class S8DestructionRequest : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    public Guid? PharmacyId { get; set; }

    [Required]
    [StringLength(20)]
    public string RequestType { get; set; } = null!;

    public Guid RequestedBy { get; set; }

    public DateTime? RequestedDate { get; set; }

    /// <summary>FK → User (lives in UserService).</summary>
    public Guid? ProcessedBy { get; set; }

    public DateTime? ActionDate { get; set; }

    public int? ActionTime { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }
}
