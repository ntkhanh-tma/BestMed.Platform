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
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user is null
            ? Results.NotFound()
            : Results.Ok(user.ToDetailDto());
    }

    private static async Task<IResult> GetByExternalIdAsync(
        string externalId,
        ReadOnlyUserDbContext db,
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.ExternalUserId == externalId, cancellationToken);

        return user is null
            ? Results.NotFound()
            : Results.Ok(user.ToDetailDto());
    }

    private static async Task<IResult> QueryAsync(
        [AsParameters] UserQueryParameters query,
        ReadOnlyUserDbContext db,
        CancellationToken cancellationToken)
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

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateUserRequest request,
        UserDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        CancellationToken cancellationToken)
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

        // Pattern: Service Bus (async) — notify downstream services only when active status changes.
        // HTTP is not appropriate here as there is no specific consumer to call synchronously.
        if (request.IsActive.HasValue && user.IsActive != previousIsActive)
        {
            await eventPublisher.PublishAsync(new UserStatusChangedEvent
            {
                UserId = user.Id,
                IsActive = user.IsActive ?? false,
                Status = user.Status
            }, cancellationToken);
        }

        return Results.Ok(user.ToDto());
    }

    private static async Task<IResult> BulkUpdateAsync(
        [FromBody] BulkUpdateUsersRequest request,
        UserDbContext db,
        IOutputCacheStore cache,
        IEventPublisher eventPublisher,
        CancellationToken cancellationToken)
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

            // Track users whose active status actually changed for event publishing.
            if (item.IsActive.HasValue && user.IsActive != previousIsActive)
                statusChangedUsers.Add((user.Id, user.IsActive ?? false, user.Status));
        }

        await db.SaveChangesAsync(cancellationToken);
        await cache.EvictByTagAsync(Extensions.CacheTagUsers, cancellationToken);

        // Pattern: Service Bus (async) — one event per user whose status changed.
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

        return Results.Ok(result);
    }
}
