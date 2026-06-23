using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.ResidentService.Entities;

/// <summary>
/// Represents a medication profile (chart) belonging to a Resident.
/// A resident can have multiple profiles (active, locked, historical).
/// </summary>
[Table("MedProfile")]
public class MedProfile : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid ResidentId { get; set; }

    /// <summary>Active / Locked / PendingChart / ForceDeleted / etc.</summary>
    [StringLength(30)]
    public string? Status { get; set; }

    public bool IsActive { get; set; }

    /// <summary>Profile is scheduled for forced removal.</summary>
    public bool? IsForcedDelete { get; set; }

    /// <summary>Lock state string returned to the client (e.g. "Locked", "Unlocked").</summary>
    [StringLength(30)]
    public string? LockStatus { get; set; }

    public Guid? LockedById { get; set; }

    [StringLength(200)]
    public string? LockedByName { get; set; }

    public DateTime? LockedAt { get; set; }

    /// <summary>Used by the optimistic concurrency check in CheckLockForMedProfile.</summary>
    public DateTime? LastChangedDate { get; set; }

    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public Guid? LastUpdatedBy { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation properties
    public Resident? Resident { get; set; }
}
