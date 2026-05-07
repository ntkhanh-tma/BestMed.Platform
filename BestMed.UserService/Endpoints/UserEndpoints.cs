using BestMed.Common.Constants;
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
        UserDbContext db,
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user is null
            ? Results.NotFound()
            : Results.Ok(user.ToDetailDto());
    }

    private static async Task<IResult> GetByExternalIdAsync(
        string externalId,
        UserDbContext db,
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.ExternalId == externalId, cancellationToken);

        return user is null
            ? Results.NotFound()
            : Results.Ok(user.ToDetailDto());
    }

    private static async Task<IResult> QueryAsync(
        [AsParameters] UserQueryParameters query,
        UserDbContext db,
        CancellationToken cancellationToken)
    {
        var queryable = db.Users.AsNoTracking().AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.Email))
            queryable = queryable.Where(u => u.Email.Contains(query.Email));

        if (!string.IsNullOrWhiteSpace(query.FirstName))
            queryable = queryable.Where(u => u.FirstName != null && u.FirstName.Contains(query.FirstName));

        if (!string.IsNullOrWhiteSpace(query.LastName))
            queryable = queryable.Where(u => u.LastName != null && u.LastName.Contains(query.LastName));

        if (query.IsActive.HasValue)
            queryable = queryable.Where(u => u.IsActive == query.IsActive.Value);

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
                ? queryable.OrderBy(u => u.CreatedAt)
                : queryable.OrderByDescending(u => u.CreatedAt)
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
        CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([id], cancellationToken);
        if (user is null) return Results.NotFound();

        if (request.Email is not null) user.Email = request.Email;
        if (request.FirstName is not null) user.FirstName = request.FirstName;
        if (request.LastName is not null) user.LastName = request.LastName;
        if (request.PhoneNumber is not null) user.PhoneNumber = request.PhoneNumber;
        if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync(cancellationToken);
        await cache.EvictByTagAsync(Extensions.CacheTagUsers, cancellationToken);
        return Results.Ok(user.ToDto());
    }

    private static async Task<IResult> BulkUpdateAsync(
        [FromBody] BulkUpdateUsersRequest request,
        UserDbContext db,
        IOutputCacheStore cache,
        CancellationToken cancellationToken)
    {
        var ids = request.Users.Select(u => u.Id).ToList();
        var users = await db.Users
            .Where(u => ids.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var notFoundIds = new List<Guid>();

        foreach (var item in request.Users)
        {
            if (!users.TryGetValue(item.Id, out var user))
            {
                notFoundIds.Add(item.Id);
                continue;
            }

            if (item.Email is not null) user.Email = item.Email;
            if (item.FirstName is not null) user.FirstName = item.FirstName;
            if (item.LastName is not null) user.LastName = item.LastName;
            if (item.PhoneNumber is not null) user.PhoneNumber = item.PhoneNumber;
            if (item.IsActive.HasValue) user.IsActive = item.IsActive.Value;
        }

        await db.SaveChangesAsync(cancellationToken);
        await cache.EvictByTagAsync(Extensions.CacheTagUsers, cancellationToken);

        var result = new BulkOperationResult
        {
            Succeeded = request.Users.Count - notFoundIds.Count,
            NotFound = notFoundIds.Count,
            NotFoundIds = notFoundIds
        };

        return Results.Ok(result);
    }
}
