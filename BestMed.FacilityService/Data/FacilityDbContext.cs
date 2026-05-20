using BestMed.Data;
using BestMed.FacilityService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.FacilityService.Data;

/// <summary>
/// Database-first DbContext for read-write operations on the Facility database.
/// </summary>
public partial class FacilityDbContext : BestMedDbContext
{
    public FacilityDbContext(DbContextOptions<FacilityDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Facility> Facilities { get; set; } = null!;
    public virtual DbSet<Section> Sections { get; set; } = null!;
    public virtual DbSet<UserFacility> UserFacilities { get; set; } = null!;
    public virtual DbSet<DoseRound> DoseRounds { get; set; } = null!;
    public virtual DbSet<FacilityDoseConfig> FacilityDoseConfigs { get; set; } = null!;
    public virtual DbSet<FacilityDoseFilterConfig> FacilityDoseFilterConfigs { get; set; } = null!;
    public virtual DbSet<FacilityBulkPackGenerateRange> FacilityBulkPackGenerateRanges { get; set; } = null!;
    public virtual DbSet<BESTtrackFacilityConfig> BESTtrackFacilityConfigs { get; set; } = null!;
    public virtual DbSet<WeeklyBulkRun> WeeklyBulkRuns { get; set; } = null!;
    public virtual DbSet<HomeCareBulkDoseRoundGenerateRange> HomeCareBulkDoseRoundGenerateRanges { get; set; } = null!;
    public virtual DbSet<S8DestructionDrug> S8DestructionDrugs { get; set; } = null!;
    public virtual DbSet<S8DestructionRequest> S8DestructionRequests { get; set; } = null!;
    public virtual DbSet<ReportMedicationSummary> ReportMedicationSummaries { get; set; } = null!;
    public virtual DbSet<ReportMedicationSummarySection> ReportMedicationSummarySections { get; set; } = null!;
    public virtual DbSet<UTILog> UTILogs { get; set; } = null!;
    public virtual DbSet<BESTMEDSupplyPharmacy> BESTMEDSupplyPharmacies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FacilityDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
