using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[Facility] table — root aggregate.
/// </summary>
[Table("Facility")]
public class Facility : IEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>FK → Pharmacy (lives in PharmacyService).</summary>
    public Guid PharmacyId { get; set; }

    /// <summary>FK → StateTimeZone.</summary>
    public Guid? StateTimeZoneId { get; set; }

    /// <summary>Active status as int (not bool). Default 1.</summary>
    public int Active { get; set; } = 1;

    [StringLength(20)]
    public string? FacilityType { get; set; }

    [StringLength(100)]
    public string? ContactName { get; set; }

    [StringLength(20)]
    public string? FredCode { get; set; }

    [StringLength(10)]
    public string? RacId { get; set; }

    [StringLength(200)]
    public string? Address1 { get; set; }

    [StringLength(100)]
    public string? Address2 { get; set; }

    [StringLength(20)]
    public string? Suburb { get; set; }

    [StringLength(20)]
    public string? State { get; set; }

    [StringLength(10)]
    public string? PostCode { get; set; }

    [StringLength(20)]
    public string? Country { get; set; }

    [StringLength(15)]
    public string? Phone { get; set; }

    [StringLength(20)]
    public string? Fax { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    public string? IPAddress { get; set; }

    public string? IPDescription { get; set; }

    public string? GeoLocations { get; set; }

    public double? GeoRadius { get; set; }

    [StringLength(11)]
    public string? ABN { get; set; }

    [StringLength(20)]
    public string? HPIONumber { get; set; }

    public string? HPIOStatus { get; set; }

    public Guid? QuarterlyGroupId { get; set; }

    public Guid? AfterHoursServiceId { get; set; }

    public bool? EnablePasswordAging { get; set; }

    public int? PasswordAging { get; set; }

    public bool ActiveDirectoryEnabled { get; set; }

    public Guid? TenantId { get; set; }

    public DateTime? ENRMCActivationDate { get; set; }

    public long? SeqNumberChart { get; set; }

    public bool? Profit { get; set; }

    [StringLength(200)]
    public string? Region { get; set; }

    public Guid? SourceFacilityId { get; set; }

    public string? Guidelines { get; set; }

    public DateTime? PDSActivationDate { get; set; }

    [StringLength(20)]
    public string? SSOOption { get; set; }

    public bool? EnableTrustedSSO { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public Guid? LastUpdatedBy { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation properties
    public ICollection<Section> Sections { get; set; } = [];
    public ICollection<UserFacility> UserFacilities { get; set; } = [];
    public ICollection<DoseRound> DoseRounds { get; set; } = [];
    public ICollection<FacilityDoseConfig> FacilityDoseConfigs { get; set; } = [];
    public ICollection<FacilityDoseFilterConfig> FacilityDoseFilterConfigs { get; set; } = [];
    public ICollection<FacilityBulkPackGenerateRange> FacilityBulkPackGenerateRanges { get; set; } = [];
    public ICollection<BESTtrackFacilityConfig> BESTtrackFacilityConfigs { get; set; } = [];
    public ICollection<WeeklyBulkRun> WeeklyBulkRuns { get; set; } = [];
    public ICollection<HomeCareBulkDoseRoundGenerateRange> HomeCareBulkDoseRoundGenerateRanges { get; set; } = [];
    public ICollection<S8DestructionDrug> S8DestructionDrugs { get; set; } = [];
    public ICollection<S8DestructionRequest> S8DestructionRequests { get; set; } = [];
    public ICollection<BESTMEDSupplyPharmacy> BESTMEDSupplyPharmacies { get; set; } = [];
}
