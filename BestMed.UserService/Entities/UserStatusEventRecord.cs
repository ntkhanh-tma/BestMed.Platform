using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.UserService.Entities;

/// <summary>
/// Append-only event stream for the selected User status operation.
/// The event stream acts as the source of truth for status changes and can be replayed.
/// </summary>
[Table("UserStatusEvent")]
public sealed class UserStatusEventRecord
{
    [Key]
    public Guid EventId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public int Version { get; set; }

    [Required]
    [StringLength(50)]
    public string EventType { get; set; } = null!;

    public bool? IsActive { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public DateTime OccurredAt { get; set; }
}