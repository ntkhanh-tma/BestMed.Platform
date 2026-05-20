using BestMed.Common.Constants;
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

    public static void ApplyTo(this UpdateRoleRequest request, Role role)
    {
        if (request.RoleCode is not null) role.RoleCode = request.RoleCode;
        if (request.RoleName is not null) role.RoleName = request.RoleName;
        if (request.Description is not null) role.Description = request.Description;
        if (request.UserTypeId.HasValue) role.UserTypeId = request.UserTypeId.Value;
        if (request.NormalizedRole is not null) role.NormalizedRole = request.NormalizedRole;
    }

    public static IQueryable<Role> ApplyFilters(this IQueryable<Role> queryable, RoleQueryParameters query)
    {
        if (!string.IsNullOrWhiteSpace(query.RoleCode))
            queryable = queryable.Where(r => r.RoleCode != null && r.RoleCode.Contains(query.RoleCode));

        if (!string.IsNullOrWhiteSpace(query.RoleName))
            queryable = queryable.Where(r => r.RoleName != null && r.RoleName.Contains(query.RoleName));

        if (query.UserTypeId.HasValue)
            queryable = queryable.Where(r => r.UserTypeId == query.UserTypeId.Value);

        return queryable;
    }

    public static IQueryable<Role> ApplySorting(this IQueryable<Role> queryable, RoleQueryParameters query)
    {
        var asc = SortDirection.IsAscending(query.SortDirection);
        return query.SortBy?.ToLowerInvariant() switch
        {
            "rolecode" => asc ? queryable.OrderBy(r => r.RoleCode) : queryable.OrderByDescending(r => r.RoleCode),
            _ => asc ? queryable.OrderBy(r => r.RoleName) : queryable.OrderByDescending(r => r.RoleName)
        };
    }
}
