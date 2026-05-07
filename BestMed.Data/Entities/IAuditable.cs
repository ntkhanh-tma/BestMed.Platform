namespace BestMed.Data.Entities;

/// <summary>
/// Implement on entities that track creation and modification timestamps.
/// <see cref="BestMedDbContext"/> automatically sets these on <c>SaveChanges</c>.
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
