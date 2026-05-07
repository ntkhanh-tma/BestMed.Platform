using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.UserService.Entities;

/// <summary>
/// Represents the [dbo].[Users] table. Database-first — do not modify manually.
/// Re-scaffold when schema changes.
/// </summary>
[Table("Users")]
public partial class User : IEntity, IAuditable
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(256)]
    public string ExternalId { get; set; } = null!;

    [Required]
    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(128)]
    public string? FirstName { get; set; }

    [StringLength(128)]
    public string? LastName { get; set; }

    [StringLength(64)]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public virtual ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
}
