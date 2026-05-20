using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;
using BestMed.Common.Models;
using BestMed.UserService.Data;
using BestMed.UserService.DTOs;
using BestMed.UserService.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BestMed.UserService.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetUserById")
            .WithDescription("Get a single user by ID including addresses")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/external/{externalId}", GetByExternalIdAsync)
            .WithName("GetUserByExternalId")
            .WithDescription("Get a single user by external identity provider ID")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", QueryAsync)
            .WithName("QueryUsers")
            .WithDescription("Search and filter users with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateUser")
            .WithDescription("Update a single user")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPut("/bulk", BulkUpdateAsync)
            .WithName("BulkUpdateUsers")
            .WithDescription("Update multiple users in a single request")
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        return routes;
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        ReadOnlyUserDbContext db,
        ILogger<UserDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await db.Users
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

    private static async Task<IResult> GetByExternalIdAsync(
        string externalId,
        ReadOnlyUserDbContext db,
        ILogger<UserDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await db.Users
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

    private static async Task<IResult> QueryAsync(
        [AsParameters] UserQueryParameters query,
        ReadOnlyUserDbContext db,
        ILogger<UserDbContext> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var queryable = db.Users
                .ApplyFilters(query)
                .ApplySorting(query);

        var totalCount = await queryable.CountAsync(cancellationToken);

        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => u.ToDto())
            .ToListAsync(cancellationToken);

        var response = new PagedResponse<UserDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };

        return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error querying users");
            return Results.Problem("An error occurred while querying users.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateUserRequest request,
        UserDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        ILogger<UserDbContext> logger,
        CancellationToken cancellationToken)
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

    private static async Task<IResult> BulkUpdateAsync(
        [FromBody] BulkUpdateUsersRequest request,
        UserDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        ILogger<UserDbContext> logger,
        CancellationToken cancellationToken)
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
