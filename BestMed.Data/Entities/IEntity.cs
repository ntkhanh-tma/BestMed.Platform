namespace BestMed.Data.Entities;

/// <summary>
/// Marker interface for all entities with a GUID primary key.
/// </summary>
public interface IEntity
{
    Guid Id { get; set; }
}
