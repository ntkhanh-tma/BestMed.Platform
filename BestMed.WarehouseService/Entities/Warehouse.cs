using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.WarehouseService.Entities;

/// <summary>
/// Represents the [dbo].[Warehouse] table. Database-first — do not modify manually.
/// Re-scaffold when schema changes.
/// </summary>
[Table("Warehouse")]
public partial class Warehouse : IEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? Address1 { get; set; }

    [StringLength(50)]
    public string? Address2 { get; set; }

    [StringLength(50)]
    public string? Suburb { get; set; }

    [StringLength(50)]
    public string? State { get; set; }

    [StringLength(6)]
    public string? PostCode { get; set; }

    [StringLength(30)]
    public string? Country { get; set; }

    [StringLength(30)]
    public string? ContactName { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(20)]
    public string? Fax { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    public string? IPAddress { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public Guid? LastUpdatedBy { get; set; }

    [StringLength(150)]
    public string? XMLUserPassword { get; set; }

    [StringLength(100)]
    public string? XMLUserName { get; set; }

    public Guid? StateTimeZoneId { get; set; }

    public string? SachetRobotTypeId { get; set; }

    public string? GeoLocations { get; set; }

    public double? GeoRadius { get; set; }

    [StringLength(500)]
    public string? IPDescription { get; set; }

    public Guid? NewUserAttachmentId { get; set; }

    [StringLength(11)]
    public string? ABN { get; set; }

    public Guid? CheckingMachineType { get; set; }

    public string? BlisterRobotTypeId { get; set; }

    public bool IsMultiSite { get; set; }

    public bool? EnablePasswordAging { get; set; }

    public int? PasswordAging { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ClusteredKey { get; set; }

    public bool RestrictPreferredBrand { get; set; }

    public bool? HasThirdPartyPacking { get; set; }

    public bool? PharmacyToInsert { get; set; }

    public string? YuyamaModelId { get; set; }

    public virtual ICollection<WarehouseBankDetail> BankDetails { get; set; } = [];
    public virtual ICollection<WarehouseDocument> Documents { get; set; } = [];
    public virtual ICollection<WarehouseHoliday> Holidays { get; set; } = [];
    public virtual ICollection<WarehouseRobot> Robots { get; set; } = [];
}
