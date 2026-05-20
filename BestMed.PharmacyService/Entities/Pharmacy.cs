using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[Pharmacy] table — root aggregate.
/// </summary>
[Table("Pharmacy")]
public class Pharmacy : IEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    public Guid? WarehouseId { get; set; }

    public Guid? StateTimeZoneId { get; set; }

    public bool Active { get; set; } = true;

    public int? PharmacyType { get; set; }

    [StringLength(100)]
    public string? Address1 { get; set; }

    [StringLength(100)]
    public string? Address2 { get; set; }

    [StringLength(50)]
    public string? Suburb { get; set; }

    [StringLength(30)]
    public string? State { get; set; }

    [StringLength(4)]
    public string? PostCode { get; set; }

    [StringLength(30)]
    public string? Country { get; set; }

    [StringLength(100)]
    public string? ShippingAddress1 { get; set; }

    [StringLength(100)]
    public string? ShippingAddress2 { get; set; }

    [StringLength(30)]
    public string? ShippingState { get; set; }

    [StringLength(50)]
    public string? ShippingSuburb { get; set; }

    [StringLength(4)]
    public string? ShippingPostCode { get; set; }

    public bool IsUseBillingAddress { get; set; } = true;

    [StringLength(200)]
    public string? BillingName { get; set; }

    public Guid? BillingContactId { get; set; }

    [StringLength(50)]
    public string? ContactName { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(20)]
    public string? Fax { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? OutOfHours { get; set; }

    [StringLength(11)]
    public string? ABN { get; set; }

    [StringLength(20)]
    public string? PharmacyApprovalNumber { get; set; }

    [StringLength(20)]
    public string? HPIONumber { get; set; }

    public string? HPIOStatus { get; set; }

    public bool? HasPackingFacility { get; set; }

    public bool? S8DrugPackingAllowed { get; set; }

    public bool IsMultiSite { get; set; }

    public bool IsInDispenseMigration { get; set; }

    public DateTime? LastDispenseMigrationDate { get; set; }

    public bool AutoSyncDispenseResident { get; set; } = true;

    public int? DispenseSystemType { get; set; }

    public int? FinancialType { get; set; }

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }

    public decimal? Discount { get; set; }

    public int? DaysInvoiceTerms { get; set; }

    public bool? EnablePasswordAging { get; set; }

    public int? PasswordAging { get; set; }

    [StringLength(14)]
    public string? WorkingDays { get; set; }

    public string? GeoLocations { get; set; }

    public double? GeoRadius { get; set; }

    public string? IPAddress { get; set; }

    [StringLength(500)]
    public string? IPDescription { get; set; }

    [StringLength(100)]
    public string? XMLUserName { get; set; }

    [StringLength(150)]
    public string? XMLUserPassword { get; set; }

    [StringLength(100)]
    public string? DropboxToken { get; set; }

    [StringLength(200)]
    public string? FredNXTAccessToken { get; set; }

    public DateTime? LastFredNXTResidentSyncDate { get; set; }

    public DateTime? LastFredNXTScriptSyncDate { get; set; }

    public DateTime? LastFredNXTPrescriberSyncDate { get; set; }

    public bool BCPChartGenerationEnabled { get; set; }

    public bool BCPSigningSheetGenerationEnabled { get; set; }

    public bool? EnablePackScheduleAPI { get; set; }

    public DateTime? PackScheduleAPIEnabledDate { get; set; }

    public bool EnableDashboard { get; set; } = true;

    public bool? EnableDistributedPacking { get; set; }

    public bool? IsBestpackCore { get; set; }

    public bool HomeCareExclude { get; set; }

    public bool? AllowPreferredBrandSubstitution { get; set; }

    public bool? AllowAllClaims { get; set; }

    public bool? IsAutoFacilityReport { get; set; }

    public int? AutoFacilityReportGenerateDate { get; set; }

    public bool? IsAutoComplianceReport { get; set; }

    public bool? IsAutoBulkMedChart { get; set; }

    public bool HasCPIClause { get; set; }

    public bool? EnableAddMoveResidentViaBpack { get; set; }

    public DateOnly? ProgrammeJoinedDate { get; set; }

    [StringLength(20)]
    public string? Tier { get; set; }

    public string? MedicationTrackingReturnedReason { get; set; }

    [StringLength(150)]
    public string? S8DestructionEmail { get; set; }

    public bool? UpdateNrmcCompliantForAudit { get; set; }

    public bool? ArchiveFredUsedRepeat { get; set; }

    public Guid? PriceModel { get; set; }

    public Guid? CheckingMachineType { get; set; }

    public string? TermsAndConditions { get; set; }

    [StringLength(15)]
    public string? TermsAndConditionsType { get; set; }

    [StringLength(10)]
    public string? ResidentsFacilityCode { get; set; }

    [StringLength(25)]
    public string? ClaimName { get; set; }

    [StringLength(25)]
    public string? ClaimQualification { get; set; }

    public string? SachetRobotTypeId { get; set; }

    public string? BlisterRobotTypeId { get; set; }

    public string? YuyamaModelId { get; set; }

    public DateOnly? SachetPackingMachineInstalledDate { get; set; }

    [StringLength(50)]
    public string? SachetPackingMachineAssetNumber { get; set; }

    public DateOnly? SachetPackingMachineInstalledDateSecond { get; set; }

    [StringLength(50)]
    public string? SachetPackingMachineAssetNumberSecond { get; set; }

    public DateOnly? BlisterPackingMachineInstalledDate { get; set; }

    [StringLength(50)]
    public string? BlisterPackingMachineAssetNumber { get; set; }

    public DateOnly? BlisterPackingMachineInstalledDateSecond { get; set; }

    [StringLength(50)]
    public string? BlisterPackingMachineAssetNumberSecond { get; set; }

    public DateOnly? CheckingMachineInstalledDate { get; set; }

    [StringLength(50)]
    public string? CheckingMachineAssetNumber { get; set; }

    public DateOnly? CheckingMachineInstalledDateSecond { get; set; }

    [StringLength(50)]
    public string? CheckingMachineAssetNumberSecond { get; set; }

    public decimal? CheckingSoftwareMaintenanceFee { get; set; }

    public decimal? ResidentFacilityFeeLessThanOrEqualTo1000 { get; set; }

    public decimal? ResidentFacilityFeeGreaterThan1000 { get; set; }

    public decimal? BESTpackCommunityFee { get; set; }

    public decimal? BESTdoctorResidentFee { get; set; }

    public decimal? BESTpackMonthlySupportFee { get; set; }

    public decimal? BESTpackDoctorIntegrationFee { get; set; }

    [StringLength(500)]
    public string? OtherInformation { get; set; }

    public long ClusteredKey { get; set; }

    // ── Audit columns ──
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public Guid? LastUpdatedBy { get; set; }

    // ── Navigation properties ──
    public ICollection<UserPharmacy> UserPharmacies { get; set; } = [];
    public ICollection<BCPSetting> BCPSettings { get; set; } = [];
    public ICollection<BESTMEDSupplyPharmacy> BESTMEDSupplyPharmacies { get; set; } = [];
    public ICollection<PackRequest> PackRequests { get; set; } = [];
    public ICollection<BatchReferenceFileBuilder> BatchReferenceFileBuilders { get; set; } = [];
    public ICollection<PharmacyInvoiceDocument> PharmacyInvoiceDocuments { get; set; } = [];
    public ICollection<QuarterlyPharmacyGroup> QuarterlyPharmacyGroups { get; set; } = [];
    public ICollection<Facility> Facilities { get; set; } = [];
}
