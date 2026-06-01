using BestMed.UserService.DTOs;
using BestMed.UserService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.UserService.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", (Guid id, IUserService svc, CancellationToken ct)
                => svc.GetByIdAsync(id, ct))
            .WithName("GetUserById")
            .WithDescription("Get a single user by ID including addresses")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/external/{externalId}", (string externalId, IUserService svc, CancellationToken ct)
                => svc.GetByExternalIdAsync(externalId, ct))
            .WithName("GetUserByExternalId")
            .WithDescription("Get a single user by external identity provider ID")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        group.MapGet("/", ([AsParameters] UserQueryParameters query, IUserService svc, CancellationToken ct)
                => svc.QueryAsync(query, ct))
            .WithName("QueryUsers")
            .WithDescription("Search and filter users with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        group.MapPut("/{id:guid}", (Guid id, [FromBody] UpdateUserRequest request, IUserService svc, CancellationToken ct)
                => svc.UpdateAsync(id, request, ct))
            .WithName("UpdateUser")
            .WithDescription("Update a single user")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPut("/{id:guid}/status", (Guid id, [FromBody] UpdateUserStatusRequest request, IUserService svc, CancellationToken ct)
                => svc.UpdateStatusAsync(id, request, ct))
            .WithName("UpdateUserStatus")
            .WithDescription("Update the user status using the event-sourced status stream")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        group.MapPut("/bulk", ([FromBody] BulkUpdateUsersRequest request, IUserService svc, CancellationToken ct)
                => svc.BulkUpdateAsync(request, ct))
            .WithName("BulkUpdateUsers")
            .WithDescription("Update multiple users in a single request")
            .RequireRateLimiting(Extensions.RateLimitHeavy);

        return routes;
    }
}

