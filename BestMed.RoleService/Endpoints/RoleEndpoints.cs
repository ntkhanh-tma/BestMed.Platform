using BestMed.Common.Constants;
using BestMed.Common.Models;
using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.RoleService.Data;
using BestMed.RoleService.DTOs;
using BestMed.RoleService.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.RoleService.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/roles")
            .WithTags("Roles")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetRoleById")
            .WithDescription("Get a single role by ID")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", QueryAsync)
            .WithName("QueryRoles")
            .WithDescription("Search and filter roles with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateRole")
            .WithDescription("Update a single role")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        ReadOnlyRoleDbContext db,
        CancellationToken cancellationToken)
    {
        var role = await db.Roles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return role is null
            ? Results.NotFound()
            : Results.Ok(role.ToDto());
    }

    private static async Task<IResult> QueryAsync(
        [AsParameters] RoleQueryParameters query,
        ReadOnlyRoleDbContext db,
        CancellationToken cancellationToken)
    {
        var queryable = db.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.RoleCode))
            queryable = queryable.Where(r => r.RoleCode != null && r.RoleCode.Contains(query.RoleCode));

        if (!string.IsNullOrWhiteSpace(query.RoleName))
            queryable = queryable.Where(r => r.RoleName != null && r.RoleName.Contains(query.RoleName));

        if (query.UserTypeId.HasValue)
            queryable = queryable.Where(r => r.UserTypeId == query.UserTypeId.Value);

        var asc = SortDirection.IsAscending(query.SortDirection);
        queryable = query.SortBy?.ToLowerInvariant() switch
        {
            "rolecode" => asc
                ? queryable.OrderBy(r => r.RoleCode)
                : queryable.OrderByDescending(r => r.RoleCode),
            _ => asc
                ? queryable.OrderBy(r => r.RoleName)
                : queryable.OrderByDescending(r => r.RoleName)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => r.ToDto())
            .ToListAsync(cancellationToken);

        return Results.Ok(new PagedResponse<RoleDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateRoleRequest request,
        RoleDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        CancellationToken cancellationToken)
    {
        var role = await db.Roles.FindAsync([id], cancellationToken);
        if (role is null) return Results.NotFound();

        if (request.RoleCode is not null) role.RoleCode = request.RoleCode;
        if (request.RoleName is not null) role.RoleName = request.RoleName;
        if (request.Description is not null) role.Description = request.Description;
        if (request.UserTypeId.HasValue) role.UserTypeId = request.UserTypeId.Value;
        if (request.NormalizedRole is not null) role.NormalizedRole = request.NormalizedRole;

        await db.SaveChangesAsync(cancellationToken);
        await cache.EvictByTagAsync("roles", cancellationToken);

        // Notify other services (e.g. UserService) that role data has changed.
        // Pattern: Service Bus (async) — fire-and-forget, consumers invalidate their caches.
        await eventPublisher.PublishAsync(new RoleUpdatedEvent
        {
            RoleId = role.Id,
            RoleName = role.RoleName,
            NormalizedRole = role.NormalizedRole
        }, cancellationToken);

        return Results.Ok(role.ToDto());
    }
}
