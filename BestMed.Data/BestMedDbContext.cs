using BestMed.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BestMed.Data;

/// <summary>
/// Base <see cref="DbContext"/> that provides:
/// <list type="bullet">
///   <item>Automatic <see cref="IAuditable.CreatedAt"/> / <see cref="IAuditable.UpdatedAt"/> stamping.</item>
///   <item>Automatic global query filter for <see cref="ISoftDeletable"/> entities.</item>
///   <item>Soft-delete interception — <c>Remove()</c> on a soft-deletable entity sets the flag instead of hard-deleting.</item>
/// </list>
/// Derive your per-service DbContext from this class instead of <see cref="DbContext"/> directly.
/// </summary>
public abstract class BestMedDbContext : DbContext
{
    protected BestMedDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filter for every entity implementing ISoftDeletable
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                continue;

            var method = typeof(BestMedDbContext)
                .GetMethod(nameof(ApplySoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(null, [modelBuilder]);
        }
    }

    private static void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyConventions();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyConventions();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyConventions()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            // Audit timestamps
            if (entry.Entity is IAuditable auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.CreatedAt = utcNow;
                        break;
                    case EntityState.Modified:
                        auditable.UpdatedAt = utcNow;
                        break;
                }
            }

            // Soft-delete interception — convert hard-delete to flag update
            if (entry is { State: EntityState.Deleted, Entity: ISoftDeletable softDeletable })
            {
                entry.State = EntityState.Modified;
                softDeletable.IsDeleted = true;
                softDeletable.DeletedAt = utcNow;
            }
        }
    }
}
