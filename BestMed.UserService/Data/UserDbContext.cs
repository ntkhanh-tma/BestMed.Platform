using BestMed.Data;
using BestMed.UserService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.UserService.Data;

/// <summary>
/// Database-first DbContext. Scaffolded entities live in the Entities folder.
/// Inherits from <see cref="BestMedDbContext"/> for automatic audit timestamps,
/// soft-delete query filters, and other shared conventions.
/// Re-scaffold with:
///   dotnet ef dbcontext scaffold ""Name=ConnectionStrings:userdb"" Microsoft.EntityFrameworkCore.SqlServer
///       --output-dir Entities --context-dir Data --context UserDbContext --force --no-onconfiguring
/// </summary>
public partial class UserDbContext : BestMedDbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<UserAddress> UserAddresses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
