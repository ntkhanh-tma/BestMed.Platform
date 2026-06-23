using BestMed.Data;
using BestMed.ResidentService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.ResidentService.Data;

/// <summary>
/// Read-write DbContext for the ResidentService. Use only for write operations.
/// </summary>
public partial class ResidentDbContext : BestMedDbContext
{
    public ResidentDbContext(DbContextOptions<ResidentDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Resident> Residents { get; set; } = null!;
    public virtual DbSet<MedProfile> MedProfiles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResidentDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
