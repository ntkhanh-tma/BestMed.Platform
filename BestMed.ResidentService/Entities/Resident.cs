using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.ResidentService.Entities;

/// <summary>
/// Represents the [dbo].[Resident] table — root aggregate of the Resident domain.
/// Migrated from BESTmedBAT; column set inferred from ResidentController operations.
/// Adjust field names/types to match exact legacy schema if they differ.
/// </summary>
[Table("Resident")]
public class Resident : IEntity
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    /// <summary>Computed or stored display name (e.g. "Last, First").</summary>
    [StringLength(120)]
    public string? DisplayName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    /// <summary>FK → Facility (lives in FacilityService).</summary>
    public Guid FacilityId { get; set; }

    /// <summary>FK → Section (lives in FacilityService).</summary>
    public Guid? SectionId { get; set; }

    /// <summary>Active/Inactive/Discharged/etc.</summary>
    [StringLength(30)]
    public string? Status { get; set; }

    /// <summary>Medical record number / FredCode equivalent.</summary>
    [StringLength(30)]
    public string? FredCode { get; set; }

    /// <summary>True when resident is supplied by a non-BESTMED pharmacy.</summary>
    public bool? IsOtherSupplyPharmacy { get; set; }

    /// <summary>FK to the alternative supply pharmacy when IsOtherSupplyPharmacy is true.</summary>
    public Guid? AlternativeSupplyPharmacyId { get; set; }

    /// <summary>Facility-level config restricts access to this resident's profile.</summary>
    public bool? IsRestrictedByFacilityConfig { get; set; }

    /// <summary>
    /// Set when a VMC transfer is pending (BMED-10406).
    /// Cleared by UpdateResidentVMCRequireTransferField(setBackToNull=true).
    /// </summary>
    public bool? VMCRequireTransfer { get; set; }

    /// <summary>Timestamp of the last resident photo upload; null sentinel = 2000-01-01.</summary>
    public DateTime? PhotoLastUpdate { get; set; }

    /// <summary>Whether document management is enabled for this resident's facility config.</summary>
    public bool? EnableDocumentManagement { get; set; }

    /// <summary>IHI (Individual Healthcare Identifier) warning text; empty string if none or user lacks edit rights.</summary>
    [StringLength(500)]
    public string? ResidentMissedIHIWarning { get; set; }

    /// <summary>IHI number for Medicare/My Health Record integration.</summary>
    [StringLength(20)]
    public string? IHINumber { get; set; }

    /// <summary>Used to detect concurrent edits / stale reads.</summary>
    public DateTime? LastChangedDate { get; set; }

    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public Guid? LastUpdatedBy { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation properties
    public ICollection<MedProfile> MedProfiles { get; set; } = [];
}
