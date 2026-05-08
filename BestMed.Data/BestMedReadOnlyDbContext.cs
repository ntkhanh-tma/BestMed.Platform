using Microsoft.EntityFrameworkCore;

namespace BestMed.Data;

/// <summary>
/// Base DbContext for read-only database operations.
/// Disables change tracking by default and blocks any write operations.
/// Derive your per-service read-only DbContext from this class.
/// </summary>
public abstract class BestMedReadOnlyDbContext : DbContext
{
    protected BestMedReadOnlyDbContext(DbContextOptions options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new InvalidOperationException("This context is read-only. Use the read-write context for write operations.");
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("This context is read-only. Use the read-write context for write operations.");
    }
}
