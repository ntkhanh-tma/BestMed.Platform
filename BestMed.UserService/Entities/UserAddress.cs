using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.UserService.Entities;

/// <summary>
/// Represents the [dbo].[UserAddresses] table. Database-first — do not modify manually.
/// </summary>
[Table("UserAddresses")]
public partial class UserAddress : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [Required]
    [StringLength(256)]
    public string Street { get; set; } = null!;

    [StringLength(128)]
    public string? City { get; set; }

    [StringLength(64)]
    public string? State { get; set; }

    [StringLength(32)]
    public string? ZipCode { get; set; }

    [StringLength(64)]
    public string? Country { get; set; }

    public bool IsPrimary { get; set; }

    // Navigation
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}
