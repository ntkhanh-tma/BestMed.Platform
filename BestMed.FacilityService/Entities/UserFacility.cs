using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[UserFacility] table — M2M: User ↔ Facility with permissions.
/// </summary>
[Table("UserFacility")]
public class UserFacility : IEntity
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>FK → User (lives in UserService).</summary>
    public Guid UserId { get; set; }

    public Guid FacilityId { get; set; }

    [StringLength(20)]
    public string? DoctorStatus { get; set; }

    [StringLength(20)]
    public string? AccessMode { get; set; }

    public bool? AllResidentAccess { get; set; }

    public int? UserPermissions { get; set; }

    [StringLength(200)]
    public string? MedicationTrackingLocation { get; set; }

    public long? BESTtrackPermission { get; set; }

    public bool? IsBESTtrackOwner { get; set; }

    [StringLength(20)]
    public string? ConfirmationStatus { get; set; }

    public DateTime? ConfirmationRequiredDate { get; set; }

    public Guid? ConfirmedBy { get; set; }

    public DateTime? ConfirmedDate { get; set; }

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
