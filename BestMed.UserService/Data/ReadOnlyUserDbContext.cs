using BestMed.Data;
using BestMed.UserService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.UserService.Data;

/// <summary>
/// Read-only DbContext for the UserService.
/// Uses a separate connection string pointing to a read replica.
/// </summary>
public partial class ReadOnlyUserDbContext : BestMedReadOnlyDbContext
{
    public ReadOnlyUserDbContext(DbContextOptions<ReadOnlyUserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
