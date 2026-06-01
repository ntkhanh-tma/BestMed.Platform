using BestMed.RoleService.DTOs;
using BestMed.RoleService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.RoleService.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/roles")
            .WithTags("Roles")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", (Guid id, IRoleService svc, CancellationToken ct)
                => svc.GetByIdAsync(id, ct))
            .WithName("GetRoleById")
            .WithDescription("Get a single role by ID")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", ([AsParameters] RoleQueryParameters query, IRoleService svc, CancellationToken ct)
                => svc.QueryAsync(query, ct))
            .WithName("QueryRoles")
            .WithDescription("Search and filter roles with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", (Guid id, [FromBody] UpdateRoleRequest request, IRoleService svc, CancellationToken ct)
                => svc.UpdateAsync(id, request, ct))
            .WithName("UpdateRole")
            .WithDescription("Update a single role")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }
}
