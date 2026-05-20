using BestMed.Data;
using BestMed.FacilityService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.FacilityService.Data;

/// <summary>
/// Read-only DbContext for the FacilityService.
/// Uses a separate connection string pointing to a read replica.
/// </summary>
public partial class ReadOnlyFacilityDbContext : BestMedReadOnlyDbContext
{
    public ReadOnlyFacilityDbContext(DbContextOptions<ReadOnlyFacilityDbContext> options) : base(options)
    {
    }

    public DbSet<Facility> Facilities { get; set; } = null!;
    public DbSet<Section> Sections { get; set; } = null!;
    public DbSet<UserFacility> UserFacilities { get; set; } = null!;
    public DbSet<DoseRound> DoseRounds { get; set; } = null!;
    public DbSet<FacilityDoseConfig> FacilityDoseConfigs { get; set; } = null!;
    public DbSet<FacilityDoseFilterConfig> FacilityDoseFilterConfigs { get; set; } = null!;
    public DbSet<FacilityBulkPackGenerateRange> FacilityBulkPackGenerateRanges { get; set; } = null!;
    public DbSet<BESTtrackFacilityConfig> BESTtrackFacilityConfigs { get; set; } = null!;
    public DbSet<WeeklyBulkRun> WeeklyBulkRuns { get; set; } = null!;
    public DbSet<HomeCareBulkDoseRoundGenerateRange> HomeCareBulkDoseRoundGenerateRanges { get; set; } = null!;
    public DbSet<S8DestructionDrug> S8DestructionDrugs { get; set; } = null!;
    public DbSet<S8DestructionRequest> S8DestructionRequests { get; set; } = null!;
    public DbSet<ReportMedicationSummary> ReportMedicationSummaries { get; set; } = null!;
    public DbSet<ReportMedicationSummarySection> ReportMedicationSummarySections { get; set; } = null!;
    public DbSet<UTILog> UTILogs { get; set; } = null!;
    public DbSet<BESTMEDSupplyPharmacy> BESTMEDSupplyPharmacies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FacilityDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
