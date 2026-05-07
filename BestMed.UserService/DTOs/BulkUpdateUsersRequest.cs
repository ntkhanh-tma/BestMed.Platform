using System.ComponentModel.DataAnnotations;

namespace BestMed.UserService.DTOs;

/// <summary>
/// Request DTO for bulk-updating multiple users.
/// </summary>
public sealed record BulkUpdateUsersRequest
{
    [Required]
    [MinLength(1)]
    public List<BulkUpdateUserItem> Users { get; init; } = [];
}

public sealed record BulkUpdateUserItem
{
    [Required]
    public Guid Id { get; init; }

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; init; }

    [StringLength(128)]
    public string? FirstName { get; init; }

    [StringLength(128)]
    public string? LastName { get; init; }

    [StringLength(64)]
    public string? PhoneNumber { get; init; }

    public bool? IsActive { get; init; }
}
