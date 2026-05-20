using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[PackResidentRoll] table.
/// FK→ PackRequest.Id, Resident.Id (external).
/// </summary>
[Table("PackResidentRoll")]
public class PackResidentRoll : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PackRequestId { get; set; }

    public Guid ResidentId { get; set; }

    public int PackType { get; set; }

    public Guid ActiveMedProfileId { get; set; }

    public Guid? PrevMedProfileId { get; set; }

    public DateTime? ResidentPackFrom { get; set; }

    public DateTime? ResidentPackTo { get; set; }

    public int? BillPackQtyDays { get; set; }

    public bool? IsS8 { get; set; }

    public bool? IsAccepted { get; set; }

    [StringLength(255)]
    public string? RejectReason { get; set; }

    public bool? DoNotCharge { get; set; }

    public Guid? VerifiedBy { get; set; }

    public DateTime? VerifiedDate { get; set; }

    [StringLength(4)]
    public string? RollNumber { get; set; }

    public bool IsIssued { get; set; }

    public bool? Disposed { get; set; }

    public bool? ReturnedtoPharmacy { get; set; }

    public bool? Discharged { get; set; }

    public Guid? MedicationTrackingActionId { get; set; }

    public Guid? TrackedBy { get; set; }

    public DateTime? TrackedDate { get; set; }

    [StringLength(200)]
    public string? MedicationTrackingLastLocation { get; set; }

    [StringLength(200)]
    public string? MedicationTrackingCurrentLocation { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(PackRequestId))]
    public PackRequest PackRequest { get; set; } = null!;

    public ICollection<PackResidentMed> PackResidentMeds { get; set; } = [];
}
