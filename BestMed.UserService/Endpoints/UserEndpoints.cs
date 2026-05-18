using BestMed.Common.Constants;
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
            var queryable = db.Users.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.Email))
            queryable = queryable.Where(u => u.Email != null && u.Email.Contains(query.Email));

        if (!string.IsNullOrWhiteSpace(query.FirstName))
            queryable = queryable.Where(u => u.FirstName != null && u.FirstName.Contains(query.FirstName));

        if (!string.IsNullOrWhiteSpace(query.LastName))
            queryable = queryable.Where(u => u.LastName != null && u.LastName.Contains(query.LastName));

        if (query.IsActive.HasValue)
            queryable = queryable.Where(u => u.IsActive == query.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(query.Type))
            queryable = queryable.Where(u => u.Type == query.Type);

        if (!string.IsNullOrWhiteSpace(query.Status))
            queryable = queryable.Where(u => u.Status == query.Status);

        if (query.RoleId.HasValue)
            queryable = queryable.Where(u => u.Role == query.RoleId.Value);

        // Apply sorting
        var asc = SortDirection.IsAscending(query.SortDirection);
        queryable = query.SortBy?.ToLowerInvariant() switch
        {
            "email" => asc
                ? queryable.OrderBy(u => u.Email)
                : queryable.OrderByDescending(u => u.Email),
            "firstname" => asc
                ? queryable.OrderBy(u => u.FirstName)
                : queryable.OrderByDescending(u => u.FirstName),
            "lastname" => asc
                ? queryable.OrderBy(u => u.LastName)
                : queryable.OrderByDescending(u => u.LastName),
            _ => asc
                ? queryable.OrderBy(u => u.CreatedDate)
                : queryable.OrderByDescending(u => u.CreatedDate)
        };

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

            if (request.Email is not null) user.Email = request.Email;
            if (request.FirstName is not null) user.FirstName = request.FirstName;
            if (request.LastName is not null) user.LastName = request.LastName;
            if (request.PreferredName is not null) user.PreferredName = request.PreferredName;
            if (request.Salutation is not null) user.Salutation = request.Salutation;
            if (request.JobTitle is not null) user.JobTitle = request.JobTitle;
            if (request.ContactNumber is not null) user.ContactNumber = request.ContactNumber;
            if (request.Status is not null) user.Status = request.Status;
            if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;
            if (request.RoleId.HasValue) user.Role = request.RoleId.Value;
            if (request.IsReadOnlyAccess.HasValue) user.IsReadOnlyAccess = request.IsReadOnlyAccess.Value;
            user.LastUpdatedDate = DateTime.UtcNow;

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

                if (item.Email is not null) user.Email = item.Email;
                if (item.FirstName is not null) user.FirstName = item.FirstName;
                if (item.LastName is not null) user.LastName = item.LastName;
                if (item.PreferredName is not null) user.PreferredName = item.PreferredName;
                if (item.ContactNumber is not null) user.ContactNumber = item.ContactNumber;
                if (item.Status is not null) user.Status = item.Status;
                if (item.IsActive.HasValue) user.IsActive = item.IsActive.Value;
                if (item.RoleId.HasValue) user.Role = item.RoleId.Value;
                if (item.IsReadOnlyAccess.HasValue) user.IsReadOnlyAccess = item.IsReadOnlyAccess.Value;
                user.LastUpdatedDate = DateTime.UtcNow;

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
