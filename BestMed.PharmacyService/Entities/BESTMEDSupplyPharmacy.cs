using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[BESTMEDSupplyPharmacy] table — Facility–Pharmacy supply config.
/// </summary>
[Table("BESTMEDSupplyPharmacy")]
public class BESTMEDSupplyPharmacy : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PharmacyId { get; set; }

    public Guid FacilityId { get; set; }

    [Required]
    [StringLength(20)]
    public string FacilityCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public int? BulkPackDayOffset { get; set; }

    [StringLength(10)]
    public string? NonPackDeliveryDay { get; set; }

    public bool? S8DrugPackingAllowed { get; set; }

    public bool? NRMCCompliantDefault { get; set; }

    [StringLength(20)]
    public string? VariablePackType { get; set; }

    [StringLength(20)]
    public string? PrnPackType { get; set; }

    [StringLength(20)]
    public string? ShortCoursePackType { get; set; }

    [StringLength(20)]
    public string? CytotoxicPackType { get; set; }

    [StringLength(20)]
    public string? AntibioticsPackType { get; set; }

    [StringLength(20)]
    public string? AntimicrobialPackTypeShortCourse { get; set; }

    [Required]
    [StringLength(10)]
    public string S4DPackType { get; set; } = "NP";

    [StringLength(20)]
    public string? PackForm { get; set; }

    public bool? DualPackingEnabled { get; set; }

    [StringLength(20)]
    public string? DefaultPackHeaderType { get; set; }

    [StringLength(20)]
    public string? DefaultPackingLocation { get; set; }

    public bool? S8ToBePackedSeparately { get; set; }

    public bool? CytotoxicToBePackedSeparately { get; set; }

    public bool? CytostaticToBePackedSeparately { get; set; }

    public bool? VariableToBePackedSeparately { get; set; }

    public bool? S4DToBePackedSeparately { get; set; }

    public bool? DoNotCrushToBePackedSeparately { get; set; }

    public bool? PackDNCandRegularInSamePackRoll { get; set; }

    public bool? ShowMedicationOnFrontOfThePack { get; set; }

    public bool? IsBlisterStartFromBottom { get; set; }

    public int? PackedPRNExpiryPeriodInMonths { get; set; }

    public int? RegularExpiry { get; set; }

    public int? S8Expiry { get; set; }

    public int? SachetPrintTime { get; set; }

    public bool? CommunitySachetLayout { get; set; }

    public bool? UtiliseAdditionalBulkRolls { get; set; }

    public Guid? RobotTypeId { get; set; }

    public Guid? DistributedPackingRobotTypeId { get; set; }

    public Guid? YuyamaModelId { get; set; }

    public Guid? DistributedPackingYuyamaModelId { get; set; }

    public bool? AllowAliasing { get; set; }

    public DateTime? FacilityLiveDate { get; set; }

    public bool? ForceDownload { get; set; }

    public bool? ForceMedChart { get; set; }

    [StringLength(10)]
    public string? MedChangeCutoffTime { get; set; }

    [StringLength(10)]
    public string? OnlineOrderingCutoffTime { get; set; }

    public bool? PrintClaimsSeparately { get; set; }

    [StringLength(10)]
    public string? S8ReportDay { get; set; }

    [StringLength(10)]
    public string? PharmacyEyeDropDeliveryDay { get; set; }

    [StringLength(10)]
    public string? EmergencyStockPatientNumber { get; set; }

    public bool? EnableMedicationTracking { get; set; }

    [StringLength(250)]
    public string? MedicationTrackingDispatchLocation { get; set; }

    public long ClusteredKey { get; set; }

    // Audit columns
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public Guid? LastUpdatedBy { get; set; }

    // Navigation
    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy Pharmacy { get; set; } = null!;
}
