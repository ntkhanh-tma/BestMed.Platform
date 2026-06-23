namespace BestMed.ResidentService.DTOs;

/// <summary>Single medication profile item.</summary>
public sealed record MedProfileDto
{
    public Guid Id { get; init; }
    public Guid ResidentId { get; init; }
    public string? Status { get; init; }
    public bool IsActive { get; init; }
    public bool? IsForcedDelete { get; init; }
    public string? LockStatus { get; init; }
    public Guid? LockedById { get; init; }
    public string? LockedByName { get; init; }
    public DateTime? LockedAt { get; init; }
    public DateTime? LastChangedDate { get; init; }
}

/// <summary>Request body for POST /residents/med-profiles/remove-pending.</summary>
public sealed record RemovePendingChartProfileRequest
{
    public Guid MedProfileId { get; init; }
    public DateTime? LatestChangedDate { get; init; }
}

/// <summary>Response for lock-check operations.</summary>
public sealed record LockProfileDto
{
    public string? LockStatus { get; init; }
    public string? LockByName { get; init; }
    public Guid? LockById { get; init; }
    public DateTime? LockedAt { get; init; }
    public bool? IsSameMedProfileId { get; init; }
}
