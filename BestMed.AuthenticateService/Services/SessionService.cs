using System.Security.Claims;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

internal sealed class SessionService(
    IUserBusiness userBusiness,
    IJwtTokenService jwtService,
    ILogger<SessionService> logger) : ISessionService
{
    private readonly ILogger<SessionService> _logger = logger;
    // ── Accept Terms & Conditions (§4.7) ──────────────────────────────────────

    public async Task<IResult> AcceptTermsAsync(ClaimsPrincipal user, string ipAddress, CancellationToken ct)
    {
        var userRole = AuthHelpers.GetClaim(user, ClaimConstants.UserRole);
        var userId = AuthHelpers.GetClaimGuid(user, ClaimConstants.UserId);

        if (userId is null)
            return Results.Problem(detail: "InvalidToken", statusCode: StatusCodes.Status401Unauthorized);

        var accepted = UserRoleCodes.AgencyAndWitnessRoles.Contains(userRole)
            ? await userBusiness.UpdateAgencyUserTermsAndConditionAsync(true, userId.Value, ct)
            : await userBusiness.UpdateTermsAndConditionAsync(true, userId.Value, ipAddress, ct);

        if (!accepted)
            return Results.Problem(detail: "FailedToAcceptTerms", statusCode: StatusCodes.Status500InternalServerError);

        var updatedToken = jwtService.UpdateClaims(
            user, new Dictionary<string, string[]> { [ClaimConstants.IsTermsAndConditionsAccepted] = ["True"] });

        return Results.Ok(new { accepted = true, accessToken = updatedToken });
    }

    // ── Switch organisation (§4.14) ───────────────────────────────────────────

    public async Task<IResult> SwitchOrganisationAsync(
        SwitchOrganisationRequest request, ClaimsPrincipal user, CancellationToken ct)
    {
        var orgListJson = AuthHelpers.GetClaim(user, ClaimConstants.OrganisationList);
        if (!IsOrgInList(orgListJson, request.OrganisationId))
            return Results.Problem(detail: "OrganisationNotInUserList", statusCode: StatusCodes.Status403Forbidden);

        var userType = AuthHelpers.GetClaim(user, ClaimConstants.UserType);
        var userRole = AuthHelpers.GetClaim(user, ClaimConstants.UserRole);
        var realId = AuthHelpers.GetClaimGuid(user, ClaimConstants.OriginalId);
        var userId = AuthHelpers.GetClaimGuid(user, ClaimConstants.UserId);

        var claimUpdates = new Dictionary<string, string[]>
        {
            [ClaimConstants.CurrentOrganisationId] = [request.OrganisationId.ToString()]
        };

        string homePage;

        if (userType == UserTypes.Facility)
        {
            await userBusiness.UpdateLastFacilityUsedAsync(request.OrganisationId, userType, ct);

            var sections = await userBusiness.GetFacilitySectionListAsync(request.OrganisationId, ct);
            if (sections.Count > 0)
            {
                claimUpdates[ClaimConstants.CurrentSection] = [Guid.Empty.ToString()];

                if (realId.HasValue)
                {
                    var screens = await userBusiness.GetUserFeatureScreensAsync(realId.Value, request.OrganisationId, ct);
                    claimUpdates[ClaimConstants.FeatureScreen] = [.. screens];
                }
            }

            if (userRole == UserRoleCodes.FDoctor || userRole == UserRoleCodes.FDietician)
            {
                claimUpdates[ClaimConstants.FacilityId] = [request.OrganisationId.ToString()];
                homePage = AreaNames.BESTdoctor;
            }
            else if (await userBusiness.IsBESTtrackFacilityAsync(request.OrganisationId, ct)
                     || userRole == UserRoleCodes.FHealthDepartmentInspector)
            {
                homePage = AreaNames.BESTtrack;
            }
            else
            {
                homePage = AreaNames.BESTdose;
            }
        }
        else
        {
            homePage = userType switch
            {
                UserTypes.Pharmacy or UserTypes.Warehouse => AreaNames.BESTpackV2,
                UserTypes.AHService => AreaNames.BESTservice,
                _ => AreaNames.BESTdose
            };
        }

        var hpioNumber = await userBusiness.GetHpioNumberAsync(request.OrganisationId, userType, ct);
        if (!string.IsNullOrEmpty(hpioNumber))
            claimUpdates[ClaimConstants.HpioNumber] = [hpioNumber];

        if (userId.HasValue)
        {
            var userBO = new UserBO { Id = userId.Value };
            var hasAccess = await userBusiness.HasAllResidentAccessAsync(userBO, request.OrganisationId, ct);
            claimUpdates[ClaimConstants.HasAllResidentAccess] = [hasAccess.ToString()];
        }

        claimUpdates[ClaimConstants.AreaName] = [homePage];

        var updatedToken = jwtService.UpdateClaims(user, claimUpdates);
        return Results.Ok(new { accessToken = updatedToken, areaName = homePage });
    }

    // ── Switch section (§4.15) ────────────────────────────────────────────────

    public async Task<IResult> SwitchSectionAsync(
        SwitchSectionRequest request, ClaimsPrincipal user, CancellationToken ct)
    {
        var userId = AuthHelpers.GetClaimGuid(user, ClaimConstants.UserId);
        if (userId is null)
            return Results.Problem(detail: "InvalidToken", statusCode: StatusCodes.Status401Unauthorized);

        await userBusiness.UpdateSelectedSectionAsync(request.SectionId, userId.Value, ct);

        var updatedToken = jwtService.UpdateClaims(
            user, new Dictionary<string, string[]> { [ClaimConstants.CurrentSection] = [request.SectionId.ToString()] });

        return Results.Ok(new { accessToken = updatedToken, areaName = AreaNames.BESTdose });
    }

    // ── Switch to facility (§4.16) ────────────────────────────────────────────

    public async Task<IResult> SwitchToFacilityAsync(
        SwitchToFacilityRequest request, ClaimsPrincipal user, CancellationToken ct)
    {
        if (!await userBusiness.IsFacilityHasS8BookAsync(request.FacilityId, ct))
            return Results.Ok(new { areaName = string.Empty });

        var existingScreens = user.FindAll(ClaimConstants.FeatureScreen).Select(c => c.Value).ToList();
        const string bbTrack = "BB_TRACK";
        if (!existingScreens.Contains(bbTrack, StringComparer.OrdinalIgnoreCase))
            existingScreens.Add(bbTrack);

        var updatedToken = jwtService.UpdateClaims(user, new Dictionary<string, string[]>
        {
            [ClaimConstants.AreaName] = [AreaNames.BESTtrack],
            [ClaimConstants.CurrentOrganisationId] = [request.FacilityId.ToString()],
            [ClaimConstants.FeatureScreen] = [.. existingScreens],
            [ClaimConstants.CurrentSection] = [Guid.Empty.ToString()]
        });

        return Results.Ok(new { accessToken = updatedToken, areaName = AreaNames.BESTtrack });
    }

    // ── Switch doctor mode (§4.19) ────────────────────────────────────────────

    public async Task<IResult> SwitchDoctorModeAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var userRole = AuthHelpers.GetClaim(user, ClaimConstants.UserRole);
        if (userRole != UserRoleCodes.FDoctor && userRole != UserRoleCodes.FDietician)
            return Results.Problem(detail: "ForbiddenRole", statusCode: StatusCodes.Status403Forbidden);

        var userName = AuthHelpers.GetClaim(user, ClaimConstants.UserName);
        var userBO = await userBusiness.GetUserByUserIdAsync(userName, ct);
        if (userBO is null)
            return Results.Problem(detail: "UserNotFound", statusCode: StatusCodes.Status401Unauthorized);

        var identity = await userBusiness.GetUserIdentificationAsync(userBO, ct);

        var currentFacilityId = userBO.LastFacilityId
                                ?? identity.Facilities.Select(f => (Guid?)f.Id).FirstOrDefault();

        if (currentFacilityId is null)
            return Results.Problem(detail: "NoFacilityAvailable", statusCode: StatusCodes.Status400BadRequest);

        var newMode = AuthHelpers.GetClaim(user, ClaimConstants.UserMode) == "Doctor" ? "Regular" : "Doctor";
        var hasAccess = await userBusiness.HasAllResidentAccessAsync(userBO, currentFacilityId.Value, ct);
        var areaName = newMode == "Doctor" ? AreaNames.BESTdoctor : AreaNames.BESTdose;

        var updatedToken = jwtService.UpdateClaims(user, new Dictionary<string, string[]>
        {
            [ClaimConstants.CurrentOrganisationId] = [currentFacilityId.Value.ToString()],
            [ClaimConstants.FacilityId] = [currentFacilityId.Value.ToString()],
            [ClaimConstants.UserMode] = [newMode],
            [ClaimConstants.HasAllResidentAccess] = [hasAccess.ToString()],
            [ClaimConstants.AreaName] = [areaName]
        });

        return Results.Ok(new { accessToken = updatedToken, areaName });
    }

    // ── Get user facilities (§4.17) ───────────────────────────────────────────

    public async Task<IResult> GetUserFacilitiesAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var realId = AuthHelpers.GetClaimGuid(user, ClaimConstants.OriginalId);
        if (realId is null)
            return Results.Problem(detail: "InvalidToken", statusCode: StatusCodes.Status401Unauthorized);

        var currentOrgId = AuthHelpers.GetClaim(user, ClaimConstants.CurrentOrganisationId);
        var facilities = await userBusiness.GetUserFacilityAsync(realId.Value, ct);

        return Results.Ok(facilities
            .Select(f => new { id = f.Id, name = f.Name, selected = f.Id.ToString() == currentOrgId })
            .OrderBy(f => f.name));
    }

    // ── Get user facilities for in-possession report (§4.18) ──────────────────

    public async Task<IResult> GetUserFacilitiesForReportAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var realId = AuthHelpers.GetClaimGuid(user, ClaimConstants.OriginalId);
        if (realId is null)
            return Results.Problem(detail: "InvalidToken", statusCode: StatusCodes.Status401Unauthorized);

        var currentOrgId = AuthHelpers.GetClaim(user, ClaimConstants.CurrentOrganisationId);
        var facilities = await userBusiness.GetUserFacilityForInPossessionReportAsync(realId.Value, ct);

        return Results.Ok(facilities
            .Select(f => new { id = f.Id, name = f.Name, selected = f.Id.ToString() == currentOrgId })
            .OrderBy(f => f.name));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static bool IsOrgInList(string orgListJson, Guid orgId)
    {
        if (string.IsNullOrWhiteSpace(orgListJson)) return false;
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(orgListJson);
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("id", out var idProp)
                    && Guid.TryParse(idProp.GetString(), out var id)
                    && id == orgId)
                    return true;
            }
        }
        catch { /* invalid JSON — deny */ }
        return false;
    }
}
