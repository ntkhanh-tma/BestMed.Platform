using System.Security.Claims;

namespace BestMed.AuthenticateService.Services;

public interface ISsoService
{
    Task<IResult> DiscoverAsync(string userName, string ipAddress, CancellationToken ct);
    Task<IResult> CallbackAsync(ClaimsPrincipal externalUser, string ipAddress, string userAgent, string? deviceHash, CancellationToken ct);
}
