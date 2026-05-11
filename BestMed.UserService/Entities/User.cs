using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.UserService.Entities;

/// <summary>
/// Represents the [dbo].[User] table. Database-first — do not modify manually.
/// Re-scaffold when schema changes.
/// </summary>
[Table("User")]
public partial class User : IEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(100)]
    public string? Salt { get; set; }

    [StringLength(255)]
    public string? UserId { get; set; }

    [StringLength(6)]
    public string? PIN { get; set; }

    public bool? IsActive { get; set; }

    public bool? ResetPassword { get; set; }

    public Guid? PasswordResetCode { get; set; }

    public Guid Role { get; set; }

    [Required]
    [StringLength(10)]
    public string Type { get; set; } = null!;

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(50)]
    public string? Salutation { get; set; }

    [StringLength(50)]
    public string? JobTitle { get; set; }

    [StringLength(50)]
    public string? ContactNumber { get; set; }

    public bool? LockToIP { get; set; }

    public bool IsTermsAndConditionsAccepted { get; set; }

    public DateTime? TermsAndConditionsAcceptedDate { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    public int LoginFailedCount { get; set; }

    public DateTime? LastLogin { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public Guid? LastSectionUsed { get; set; }

    public Guid? LastFacilityUsed { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public Guid? LastUpdatedBy { get; set; }

    public Guid? PrescriberId { get; set; }

    public bool IsBHSStaff { get; set; }

    public DateTime? PasswordLastUpdated { get; set; }

    public bool IsExternalLogin { get; set; }

    [StringLength(500)]
    public string? ExternalUserId { get; set; }

    public bool IsBESTmedLogin { get; set; }

    public int LoginIdChangedCount { get; set; }

    public byte? SignOutRule { get; set; }

    [StringLength(13)]
    public string? AHPRANumber { get; set; }

    public short? LastMedicationHistoryTab { get; set; }

    public int? PinFailedCount { get; set; }

    public bool EmailConfirmed { get; set; }

    public bool LockoutEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    [Required]
    public string SecurityStamp { get; set; } = null!;

    public string? ConcurrencyStamp { get; set; }

    [StringLength(255)]
    public string? NormalizedEmail { get; set; }

    [StringLength(255)]
    public string? NormalizedUserId { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public bool? IsProxyAccount { get; set; }

    public string? UserQualifications { get; set; }

    public int? DeviceRegistrationPinFailedCount { get; set; }

    [StringLength(20)]
    public string? HPIINumber { get; set; }

    [StringLength(50)]
    public string? HPIIStatus { get; set; }

    public bool IsReadOnlyAccess { get; set; }

    public string? CustomRoleCheckSum { get; set; }

    public DateTime? LockTime { get; set; }

    [StringLength(50)]
    public string? IntegrationId { get; set; }

    [StringLength(100)]
    public string? IntegrationSystem { get; set; }

    public int? PinDualFailedCount { get; set; }

    [StringLength(50)]
    public string? PreferredName { get; set; }
}
