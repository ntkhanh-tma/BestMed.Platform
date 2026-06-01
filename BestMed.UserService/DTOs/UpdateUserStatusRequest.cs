using System.ComponentModel.DataAnnotations;

namespace BestMed.UserService.DTOs;

/// <summary>
/// Request DTO for the event-sourced user status operation.
/// </summary>
public sealed record UpdateUserStatusRequest
{
    public bool? IsActive { get; init; }

    [StringLength(20)]
    public string? Status { get; init; }
}