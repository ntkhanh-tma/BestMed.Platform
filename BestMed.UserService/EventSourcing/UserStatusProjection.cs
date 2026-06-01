using BestMed.UserService.Entities;

namespace BestMed.UserService.EventSourcing;

/// <summary>
/// Rebuilt status state for the selected event-sourced operation.
/// </summary>
public sealed record UserStatusProjection(bool? IsActive, string? Status)
{
    public void ApplyTo(User user)
    {
        user.IsActive = IsActive;
        user.Status = Status;
        user.LastUpdatedDate = DateTime.UtcNow;
    }
}