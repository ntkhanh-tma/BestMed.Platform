using BestMed.Data;
using BestMed.RoleService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.RoleService.Data;

/// <summary>
/// Read-only DbContext for the RoleService.
/// Uses a separate connection string pointing to a read replica.
/// </summary>
public partial class ReadOnlyRoleDbContext : BestMedReadOnlyDbContext
{
    public ReadOnlyRoleDbContext(DbContextOptions<ReadOnlyRoleDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RoleDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
