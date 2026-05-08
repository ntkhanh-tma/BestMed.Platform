using System.ComponentModel.DataAnnotations;

namespace BestMed.RoleService.DTOs;

/// <summary>
/// Request DTO for updating a role.
/// </summary>
public sealed record UpdateRoleRequest
{
    [StringLength(50)]
    public string? RoleCode { get; init; }

    [StringLength(150)]
    public string? RoleName { get; init; }

    [StringLength(250)]
    public string? Description { get; init; }

    public Guid? UserTypeId { get; init; }

    [StringLength(50)]
    public string? NormalizedRole { get; init; }
}
