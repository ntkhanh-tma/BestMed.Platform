using BestMed.Data;
using BestMed.PharmacyService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.PharmacyService.Data;

/// <summary>
/// Read-only DbContext for the PharmacyService.
/// Uses a separate connection string pointing to a read replica.
/// </summary>
public partial class ReadOnlyPharmacyDbContext : BestMedReadOnlyDbContext
{
    public ReadOnlyPharmacyDbContext(DbContextOptions<ReadOnlyPharmacyDbContext> options) : base(options)
    {
    }

    public DbSet<Pharmacy> Pharmacies { get; set; } = null!;
    public DbSet<UserPharmacy> UserPharmacies { get; set; } = null!;
    public DbSet<BCPSetting> BCPSettings { get; set; } = null!;
    public DbSet<BESTMEDSupplyPharmacy> BESTMEDSupplyPharmacies { get; set; } = null!;
    public DbSet<SupplyPharmacy> SupplyPharmacies { get; set; } = null!;
    public DbSet<SupplyPharmacySection> SupplyPharmacySections { get; set; } = null!;
    public DbSet<NonBhsUserPharmacy> NonBhsUserPharmacies { get; set; } = null!;
    public DbSet<PackRequest> PackRequests { get; set; } = null!;
    public DbSet<PackResidentRoll> PackResidentRolls { get; set; } = null!;
    public DbSet<PackResidentMed> PackResidentMeds { get; set; } = null!;
    public DbSet<BatchReferenceFileBuilder> BatchReferenceFileBuilders { get; set; } = null!;
    public DbSet<PharmacyInvoiceDocument> PharmacyInvoiceDocuments { get; set; } = null!;
    public DbSet<QuarterlyPharmacyGroup> QuarterlyPharmacyGroups { get; set; } = null!;
    public DbSet<Facility> Facilities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmacyDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
