using System.Security.Claims;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

public interface ISessionService
{
    Task<IResult> AcceptTermsAsync(ClaimsPrincipal user, string ipAddress, CancellationToken ct);
    Task<IResult> SwitchOrganisationAsync(SwitchOrganisationRequest request, ClaimsPrincipal user, CancellationToken ct);
    Task<IResult> SwitchSectionAsync(SwitchSectionRequest request, ClaimsPrincipal user, CancellationToken ct);
    Task<IResult> SwitchToFacilityAsync(SwitchToFacilityRequest request, ClaimsPrincipal user, CancellationToken ct);
    Task<IResult> SwitchDoctorModeAsync(ClaimsPrincipal user, CancellationToken ct);
    Task<IResult> GetUserFacilitiesAsync(ClaimsPrincipal user, CancellationToken ct);
    Task<IResult> GetUserFacilitiesForReportAsync(ClaimsPrincipal user, CancellationToken ct);
}
