using BestMed.UserService.Data;
using BestMed.UserService.DTOs;
using BestMed.UserService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.UserService.EventSourcing;

/// <summary>
/// Minimal event store for the selected User status operation.
/// Stores the ordered status changes and can rebuild the current projection from history.
/// </summary>
internal sealed class UserStatusEventStore(UserDbContext db)
{
    private const string EventTypeName = nameof(UpdateUserStatusRequest);

    public async Task<UserStatusProjection> AppendAsync(User user, UpdateUserStatusRequest request, CancellationToken cancellationToken)
    {
        var projection = new UserStatusProjection(
            request.IsActive ?? user.IsActive,
            request.Status ?? user.Status);

        var latestVersion = await db.UserStatusEvents
            .Where(e => e.UserId == user.Id)
            .Select(e => (int?)e.Version)
            .MaxAsync(cancellationToken) ?? 0;

        db.UserStatusEvents.Add(new UserStatusEventRecord
        {
            EventId = Guid.NewGuid(),
            UserId = user.Id,
            Version = latestVersion + 1,
            EventType = EventTypeName,
            IsActive = projection.IsActive,
            Status = projection.Status,
            OccurredAt = DateTime.UtcNow
        });

        return projection;
    }

    public async Task<UserStatusProjection?> GetCurrentStateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var lastEvent = await db.UserStatusEvents
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Version)
            .FirstOrDefaultAsync(cancellationToken);

        return lastEvent is null
            ? null
            : new UserStatusProjection(lastEvent.IsActive, lastEvent.Status);
    }
}