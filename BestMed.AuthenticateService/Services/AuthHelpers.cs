using System.Security.Claims;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

/// <summary>
/// Thin HTTP-layer utilities for extracting common values from the request context.
/// Used by endpoint delegates only — service classes receive primitives, not HttpContext.
/// </summary>
internal static class AuthHelpers
{
    internal static string GetIpAddress(HttpContext context)
        => context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
           ?? context.Connection.RemoteIpAddress?.ToString()
           ?? string.Empty;

    internal static string GetUserAgent(HttpContext context)
        => context.Request.Headers.UserAgent.ToString();

    internal static string? GetDeviceHash(HttpContext context)
        => context.Request.Headers["X-Device-Hash"].FirstOrDefault();

    internal static Guid? GetClaimGuid(ClaimsPrincipal user, string claimType)
    {
        var value = user.FindFirst(claimType)?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }

    internal static string GetClaim(ClaimsPrincipal user, string claimType)
        => user.FindFirst(claimType)?.Value ?? string.Empty;
}
