using System.ComponentModel.DataAnnotations;

namespace BestMed.UserService.DTOs;

/// <summary>
/// Request DTO for updating a single user.
/// </summary>
public sealed record UpdateUserRequest
{
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
    public string? Salutation { get; init; }

    [StringLength(50)]
    public string? JobTitle { get; init; }

    [StringLength(50)]
    public string? ContactNumber { get; init; }

    [StringLength(20)]
    public string? Status { get; init; }

    public bool? IsActive { get; init; }

    public Guid? RoleId { get; init; }

    public bool? IsReadOnlyAccess { get; init; }
}
