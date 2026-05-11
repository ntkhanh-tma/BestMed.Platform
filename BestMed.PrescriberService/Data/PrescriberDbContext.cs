using BestMed.Data;
using BestMed.PrescriberService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.PrescriberService.Data;

/// <summary>
/// Database-first DbContext for read-write operations.
/// Inherits from <see cref="BestMedDbContext"/> for automatic audit timestamps,
/// soft-delete query filters, and other shared conventions.
/// Re-scaffold with:
///   dotnet ef dbcontext scaffold "Name=ConnectionStrings:prescriberdb" Microsoft.EntityFrameworkCore.SqlServer
///       --output-dir Entities --context-dir Data --context PrescriberDbContext --force --no-onconfiguring
/// </summary>
public partial class PrescriberDbContext : BestMedDbContext
{
    public PrescriberDbContext(DbContextOptions<PrescriberDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Prescriber> Prescribers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrescriberDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
