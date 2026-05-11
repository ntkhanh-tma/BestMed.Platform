using BestMed.Data;
using BestMed.PrescriberService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.PrescriberService.Data;

/// <summary>
/// Read-only DbContext for the PrescriberService.
/// Uses a separate connection string pointing to a read replica.
/// </summary>
public partial class ReadOnlyPrescriberDbContext : BestMedReadOnlyDbContext
{
    public ReadOnlyPrescriberDbContext(DbContextOptions<ReadOnlyPrescriberDbContext> options) : base(options)
    {
    }

    public DbSet<Prescriber> Prescribers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrescriberDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
