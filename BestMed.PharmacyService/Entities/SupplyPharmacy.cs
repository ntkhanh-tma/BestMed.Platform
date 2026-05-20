using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[SupplyPharmacy] table — non-BESTmed supply pharmacies.
/// Audit columns are NOT NULL on this entity.
/// </summary>
[Table("SupplyPharmacy")]
public class SupplyPharmacy : IEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    public Guid FacilityId { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(100)]
    public string? Address1 { get; set; }

    [StringLength(100)]
    public string? Address2 { get; set; }

    [StringLength(50)]
    public string? Suburb { get; set; }

    [StringLength(50)]
    public string? State { get; set; }

    [StringLength(4)]
    public string? PostCode { get; set; }

    [StringLength(30)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(20)]
    public string? Fax { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? OutOfHours { get; set; }

    public string? IPAddress { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime LastUpdatedDate { get; set; }

    public Guid LastUpdatedBy { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    public ICollection<NonBhsUserPharmacy> NonBhsUserPharmacies { get; set; } = [];
    public ICollection<SupplyPharmacySection> SupplyPharmacySections { get; set; } = [];
}
