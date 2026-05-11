using BestMed.RoleService.DTOs;
using BestMed.RoleService.Entities;

namespace BestMed.RoleService.Mapping;

public static class RoleMappingExtensions
{
    public static RoleDto ToDto(this Role entity) => new()
    {
        Id = entity.Id,
        RoleCode = entity.RoleCode,
        RoleName = entity.RoleName,
        Description = entity.Description,
        UserTypeId = entity.UserTypeId,
        NormalizedRole = entity.NormalizedRole
    };
}
