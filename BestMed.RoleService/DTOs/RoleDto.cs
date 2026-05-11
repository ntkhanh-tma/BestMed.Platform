namespace BestMed.RoleService.DTOs;

/// <summary>
/// Response DTO for a role.
/// </summary>
public sealed record RoleDto
{
    public Guid Id { get; init; }
    public string? RoleCode { get; init; }
    public string? RoleName { get; init; }
    public string? Description { get; init; }
    public Guid? UserTypeId { get; init; }
    public string? NormalizedRole { get; init; }
}
