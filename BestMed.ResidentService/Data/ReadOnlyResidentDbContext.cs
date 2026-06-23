using BestMed.Data;
using BestMed.ResidentService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.ResidentService.Data;

/// <summary>
/// Read-only DbContext for the ResidentService. Uses a separate connection string
/// pointing at a read replica. No change tracking — all queries are AsNoTracking by default.
/// </summary>
public partial class ReadOnlyResidentDbContext : BestMedReadOnlyDbContext
{
    public ReadOnlyResidentDbContext(DbContextOptions<ReadOnlyResidentDbContext> options) : base(options)
    {
    }

    public DbSet<Resident> Residents { get; set; } = null!;
    public DbSet<MedProfile> MedProfiles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Reuse configurations defined against ResidentDbContext
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResidentDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
