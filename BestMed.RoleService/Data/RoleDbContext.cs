using BestMed.Data;
using BestMed.RoleService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.RoleService.Data;

/// <summary>
/// Database-first DbContext for read-write operations.
/// Inherits from <see cref="BestMedDbContext"/> for automatic audit timestamps,
/// soft-delete query filters, and other shared conventions.
/// Re-scaffold with:
///   dotnet ef dbcontext scaffold "Name=ConnectionStrings:roledb" Microsoft.EntityFrameworkCore.SqlServer
///       --output-dir Entities --context-dir Data --context RoleDbContext --force --no-onconfiguring
/// </summary>
public partial class RoleDbContext : BestMedDbContext
{
    public RoleDbContext(DbContextOptions<RoleDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RoleDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
