namespace BestMed.Common.Contracts;

/// <summary>
/// Shared contract DTO for role data returned by RoleService.
/// Used by consumers (e.g. UserService) without a direct project reference to RoleService.
/// </summary>
public sealed record RoleContract
{
    public Guid Id { get; init; }
    public string? RoleCode { get; init; }
    public string? RoleName { get; init; }
    public string? Description { get; init; }
    public Guid? UserTypeId { get; init; }
    public string? NormalizedRole { get; init; }
}
