using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.Common.Models;
using BestMed.UserService.Data;
using BestMed.UserService.DTOs;
using BestMed.UserService.EventSourcing;
using BestMed.UserService.Mapping;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BestMed.UserService.Services;

/// <summary>
/// Implements all user business/data logic.
/// Dependencies are injected so the class is fully unit-testable in isolation.
/// </summary>
internal sealed class UserService(
    UserDbContext db,
    ReadOnlyUserDbContext readDb,
    UserStatusEventStore statusEventStore,
    IOutputCacheStore cache,
    IEventPublisher eventPublisher,
    ILogger<UserService> logger) : IUserService
{
    public async Task<IResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var user = await readDb.Users
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            return user is null
                ? Results.NotFound()
                : Results.Ok(user.ToDetailDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user {UserId}", id);
            return Results.Problem("An error occurred while retrieving the user.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await readDb.Users
                .FirstOrDefaultAsync(u => u.ExternalUserId == externalId, cancellationToken);

            return user is null
                ? Results.NotFound()
                : Results.Ok(user.ToDetailDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user by external ID '{ExternalId}'", externalId);
            return Results.Problem("An error occurred while retrieving the user.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> QueryAsync(UserQueryParameters query, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = readDb.Users
                .ApplyFilters(query)
                .ApplySorting(query);

            var totalCount = await queryable.CountAsync(cancellationToken);
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(u => u.ToDto())
                .ToListAsync(cancellationToken);

            return Results.Ok(new PagedResponse<UserDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error querying users");
            return Results.Problem("An error occurred while querying users.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating user {UserId}", id);

        try
        {
            var user = await db.Users.FindAsync([id], cancellationToken);
            if (user is null) return Results.NotFound();

            var previousIsActive = user.IsActive;

            request.ApplyTo(user);

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync(Extensions.CacheTagUsers, cancellationToken);

            if (request.IsActive.HasValue && user.IsActive != previousIsActive)
            {
                await eventPublisher.PublishAsync(new UserStatusChangedEvent
                {
                    UserId = user.Id,
                    IsActive = user.IsActive ?? false,
                    Status = user.Status
                }, cancellationToken);
            }

            logger.LogInformation("User {UserId} updated successfully", id);
            return Results.Ok(user.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user {UserId}", id);
            return Results.Problem("An error occurred while updating the user.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> UpdateStatusAsync(Guid id, UpdateUserStatusRequest request, CancellationToken cancellationToken)
    {
        if (request.IsActive is null && request.Status is null)
            return Results.BadRequest("At least one status field must be provided.");

        logger.LogInformation("Updating user status {UserId}", id);

        try
        {
            var user = await db.Users.FindAsync([id], cancellationToken);
            if (user is null) return Results.NotFound();

            if (!request.HasStatusChanges(user))
                return Results.Ok(user.ToDto());

            var projection = await statusEventStore.AppendAsync(user, request, cancellationToken);
            projection.ApplyTo(user);

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync(Extensions.CacheTagUsers, cancellationToken);

            await eventPublisher.PublishAsync(new UserStatusChangedEvent
            {
                UserId = user.Id,
                IsActive = user.IsActive ?? false,
                Status = user.Status
            }, cancellationToken);

            logger.LogInformation("User status {UserId} updated successfully", id);
            return Results.Ok(user.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user status {UserId}", id);
            return Results.Problem("An error occurred while updating the user status.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> BulkUpdateAsync(BulkUpdateUsersRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Bulk updating {Count} user(s)", request.Users.Count);

        try
        {
            var ids = request.Users.Select(u => u.Id).ToList();
            var users = await db.Users
                .Where(u => ids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            var notFoundIds = new List<Guid>();
            var statusChangedUsers = new List<(Guid UserId, bool IsActive, string? Status)>();

            foreach (var item in request.Users)
            {
                if (!users.TryGetValue(item.Id, out var user))
                {
                    notFoundIds.Add(item.Id);
                    continue;
                }

                var previousIsActive = user.IsActive;

                item.ApplyTo(user);

                if (item.IsActive.HasValue && user.IsActive != previousIsActive)
                    statusChangedUsers.Add((user.Id, user.IsActive ?? false, user.Status));
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.EvictByTagAsync(Extensions.CacheTagUsers, cancellationToken);

            foreach (var (userId, isActive, status) in statusChangedUsers)
            {
                await eventPublisher.PublishAsync(new UserStatusChangedEvent
                {
                    UserId = userId,
                    IsActive = isActive,
                    Status = status
                }, cancellationToken);
            }

            var result = new BulkOperationResult
            {
                Succeeded = request.Users.Count - notFoundIds.Count,
                NotFound = notFoundIds.Count,
                NotFoundIds = notFoundIds
            };

            logger.LogInformation("Bulk update completed: {Succeeded} succeeded, {NotFound} not found", result.Succeeded, result.NotFound);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during bulk user update");
            return Results.Problem("An error occurred while updating users.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
