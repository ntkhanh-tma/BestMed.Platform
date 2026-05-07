namespace BestMed.Data.Entities;

/// <summary>
/// Implement on entities that support soft-delete.
/// <see cref="BestMedDbContext"/> automatically applies a global query filter
/// to exclude soft-deleted rows and sets the timestamp on delete.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
