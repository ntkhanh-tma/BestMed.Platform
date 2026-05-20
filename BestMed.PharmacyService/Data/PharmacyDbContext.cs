using BestMed.Data;
using BestMed.PharmacyService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.PharmacyService.Data;

/// <summary>
/// Database-first DbContext for read-write operations on the Pharmacy database.
/// </summary>
public partial class PharmacyDbContext : BestMedDbContext
{
    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Pharmacy> Pharmacies { get; set; } = null!;
    public virtual DbSet<UserPharmacy> UserPharmacies { get; set; } = null!;
    public virtual DbSet<BCPSetting> BCPSettings { get; set; } = null!;
    public virtual DbSet<BESTMEDSupplyPharmacy> BESTMEDSupplyPharmacies { get; set; } = null!;
    public virtual DbSet<SupplyPharmacy> SupplyPharmacies { get; set; } = null!;
    public virtual DbSet<SupplyPharmacySection> SupplyPharmacySections { get; set; } = null!;
    public virtual DbSet<NonBhsUserPharmacy> NonBhsUserPharmacies { get; set; } = null!;
    public virtual DbSet<PackRequest> PackRequests { get; set; } = null!;
    public virtual DbSet<PackResidentRoll> PackResidentRolls { get; set; } = null!;
    public virtual DbSet<PackResidentMed> PackResidentMeds { get; set; } = null!;
    public virtual DbSet<BatchReferenceFileBuilder> BatchReferenceFileBuilders { get; set; } = null!;
    public virtual DbSet<PharmacyInvoiceDocument> PharmacyInvoiceDocuments { get; set; } = null!;
    public virtual DbSet<QuarterlyPharmacyGroup> QuarterlyPharmacyGroups { get; set; } = null!;
    public virtual DbSet<Facility> Facilities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmacyDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
