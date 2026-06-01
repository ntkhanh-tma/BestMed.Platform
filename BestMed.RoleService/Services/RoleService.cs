using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.Common.Models;
using BestMed.RoleService.Data;
using BestMed.RoleService.DTOs;
using BestMed.RoleService.Mapping;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.RoleService.Services;

/// <summary>
/// Implements all role business/data logic.
/// Dependencies are injected so the class is fully unit-testable in isolation.
/// </summary>
public sealed class RoleService(
    RoleDbContext db,
    ReadOnlyRoleDbContext readDb,
    IOutputCacheStore cache,
    IEventPublisher eventPublisher,
    ILogger<RoleService> logger) : IRoleService
{
    public async Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var role = await readDb.Roles
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            return role is null
                ? Results.NotFound()
                : Results.Ok(role.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving role {RoleId}", id);
            return Results.Problem("An error occurred while retrieving the role.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> QueryAsync(RoleQueryParameters query, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = readDb.Roles
                .ApplyFilters(query)
                .ApplySorting(query);

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
        catch (Exception ex)
        {
            logger.LogError(ex, "Error querying roles");
            return Results.Problem("An error occurred while querying roles.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating role {RoleId}", id);

        try
        {
            var role = await db.Roles.FindAsync([id], cancellationToken);
            if (role is null) return Results.NotFound();

            request.ApplyTo(role);

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync("roles", cancellationToken);

            await eventPublisher.PublishAsync(new RoleUpdatedEvent
            {
                RoleId = role.Id,
                RoleName = role.RoleName,
                NormalizedRole = role.NormalizedRole
            }, cancellationToken);

            logger.LogInformation("Role {RoleId} updated successfully", id);
            return Results.Ok(role.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating role {RoleId}", id);
            return Results.Problem("An error occurred while updating the role.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
