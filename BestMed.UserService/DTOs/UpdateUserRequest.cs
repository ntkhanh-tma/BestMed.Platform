using System.ComponentModel.DataAnnotations;

namespace BestMed.UserService.DTOs;

/// <summary>
/// Request DTO for updating a single user.
/// </summary>
public sealed record UpdateUserRequest
{
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
