namespace BestMed.UserService.DTOs;

/// <summary>
/// Response DTO for a single user.
/// </summary>
public sealed record UserDto
{
    public Guid Id { get; init; }
    public string ExternalId { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
