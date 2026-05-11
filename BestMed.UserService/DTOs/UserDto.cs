namespace BestMed.UserService.DTOs;

/// <summary>
/// Summary response DTO for a user.
/// </summary>
public sealed record UserDto
{
    public Guid Id { get; init; }
    public string? UserId { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PreferredName { get; init; }
    public string? Salutation { get; init; }
    public string? JobTitle { get; init; }
    public string? ContactNumber { get; init; }
    public string Type { get; init; } = null!;
    public string? Status { get; init; }
    public bool? IsActive { get; init; }
    public Guid RoleId { get; init; }
    public Guid? PrescriberId { get; init; }
    public bool IsExternalLogin { get; init; }
    public string? ExternalUserId { get; init; }
    public DateTime? LastLogin { get; init; }
    public DateTime? CreatedDate { get; init; }
    public DateTime? LastUpdatedDate { get; init; }
}
