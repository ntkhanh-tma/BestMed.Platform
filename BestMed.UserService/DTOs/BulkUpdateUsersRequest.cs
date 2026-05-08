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
    [StringLength(255)]
    public string? Email { get; init; }

    [StringLength(50)]
    public string? FirstName { get; init; }

    [StringLength(50)]
    public string? LastName { get; init; }

    [StringLength(50)]
    public string? PreferredName { get; init; }

    [StringLength(50)]
    public string? ContactNumber { get; init; }

    [StringLength(20)]
    public string? Status { get; init; }

    public bool? IsActive { get; init; }

    public Guid? RoleId { get; init; }

    public bool? IsReadOnlyAccess { get; init; }
}
