using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[PackRequest] table.
/// FK→ Pharmacy.Id, Facility.Id (external), User.Id (external, as PackRequestBy).
/// No ClusteredKey. No audit columns.
/// </summary>
[Table("PackRequest")]
public class PackRequest : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PharmacyId { get; set; }

    public Guid FacilityId { get; set; }

    public Guid? SectionId { get; set; }

    [Required]
    [StringLength(20)]
    public string RequestNumber { get; set; } = null!;

    public DateTime PackRequestDate { get; set; }

    public Guid PackRequestBy { get; set; }

    public DateTime? PackFrom { get; set; }

    public DateTime? PackTo { get; set; }

    public int PackRequestType { get; set; }

    public int Status { get; set; }

    public int? WarehouseStatus { get; set; }

    [Required]
    [StringLength(1)]
    public string Generatedby { get; set; } = null!;

    public Guid? PackDocumentId { get; set; }

    public Guid? BillDocumentId { get; set; }

    public Guid? ProcessId { get; set; }

    public bool? IsVerified { get; set; }

    public bool? BillStatus { get; set; }

    public Guid? RobotTypeId { get; set; }

    [StringLength(255)]
    public string? Packer { get; set; }

    public DateTime? VMCReverseDate { get; set; }

    public Guid? VMCReverseBy { get; set; }

    public int? PackRequestLocationId { get; set; }

    // Navigation
    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy Pharmacy { get; set; } = null!;

    public ICollection<PackResidentRoll> PackResidentRolls { get; set; } = [];
}
