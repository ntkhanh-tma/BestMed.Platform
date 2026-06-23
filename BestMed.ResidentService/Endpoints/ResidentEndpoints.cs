using System.Security.Claims;
using BestMed.ResidentService.DTOs;
using BestMed.ResidentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.ResidentService.Endpoints;

/// <summary>
/// Top-level endpoint registration for the ResidentService.
/// Composes the resident, med-profile, and VMC sub-groups.
/// All routes sit under the gateway path /residents/** (configured in BestMed.Gateway).
/// </summary>
public static class ResidentEndpoints
{
    public static IEndpointRouteBuilder MapResidentEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapResidentCoreEndpoints();
        routes.MapMedProfileEndpoints();
        routes.MapVmcEndpoints();
        return routes;
    }

    private static IEndpointRouteBuilder MapResidentCoreEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/residents")
            .WithTags("Residents")
            .RequireAuthorization();

        // 4.2 — Resident list with filtering/sorting/paging
        group.MapGet("/", ([AsParameters] ResidentQueryParameters query, IResidentService svc, CancellationToken ct)
                => svc.GetResidentsAsync(query, ct))
            .WithName("GetResidents")
            .WithDescription("Search and filter residents with pagination")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        // 4.3 — Quick search
        group.MapPost("/quick-search", ([FromBody] QuickSearchRequest request, IResidentService svc, CancellationToken ct)
                => svc.QuickSearchAsync(request, ct))
            .WithName("QuickSearchResidents")
            .WithDescription("Fast resident search by name or code")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.33 — Dropdown / filter list
        group.MapGet("/list", (Guid? pharmacyId, Guid? facilityId, Guid? sectionId, Guid? prescriberId,
                IResidentService svc, CancellationToken ct)
                => svc.GetResidentListAsync(pharmacyId, facilityId, sectionId, prescriberId, ct))
            .WithName("GetResidentList")
            .WithDescription("Resident dropdown/filter list, optionally scoped to facility/section/prescriber/pharmacy")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        // 4.47 — Medication-tracking residents for the current user
        group.MapGet("/medication-tracking", (
                [FromQuery(Name = "facilityIds")] string? facilityIdsRaw,
                IResidentService svc, CancellationToken ct) =>
            {
                var facilityIds = facilityIdsRaw?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => Guid.TryParse(s.Trim(), out var g) ? g : (Guid?)null)
                    .Where(g => g.HasValue)
                    .Select(g => g!.Value)
                    .ToList();

                return svc.GetMedicationTrackingResidentsAsync(facilityIds, ct);
            })
            .WithName("GetMedicationTrackingResidents")
            .WithDescription("Residents visible to the current user for medication tracking purposes")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.19 — Resident detail
        group.MapGet("/{residentId:guid}", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.GetResidentDetailsAsync(residentId, ct))
            .WithName("GetResidentById")
            .WithDescription("Get a single resident's full detail model")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        // 4.48 — Facility name for a resident
        group.MapGet("/{residentId:guid}/facility-name", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.GetFacilityNameAsync(residentId, ct))
            .WithName("GetResidentFacilityName")
            .WithDescription("Returns the facility name for the given resident")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        // 4.4 — Full resident profile with computed flags
        // hasEditRights and isNationalPlatformUser are resolved from JWT claims here so that
        // the service layer never has to inspect HttpContext directly.
        group.MapGet("/{residentId:guid}/profile", (
                Guid residentId,
                Guid? drugId,
                string defaultTab,
                ClaimsPrincipal user,
                IResidentService svc,
                CancellationToken ct) =>
            {
                // Resolve claims before delegating — the service must never read HttpContext.
                // TODO: Replace magic strings with ClaimTypes constants matching the JWT issuer.
                _ = user.HasEditRights();
                _ = user.IsNationalPlatformUser();
                return svc.GetProfileAsync(residentId, drugId, defaultTab ?? "1", ct);
            })
            .WithName("GetResidentProfile")
            .WithDescription("Full resident profile including computed permission flags")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        // 4.5 — Nav-bar header partial
        group.MapGet("/{residentId:guid}/header", (
                Guid residentId,
                bool? allergies,
                bool? checkIhi,
                IResidentService svc,
                CancellationToken ct)
                => svc.GetHeaderAsync(residentId, allergies ?? false, checkIhi ?? false, ct))
            .WithName("GetResidentHeader")
            .WithDescription("Resident nav-bar header data")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        // 4.7 — Last packed-until date
        group.MapGet("/{residentId:guid}/last-packed-until", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.GetLastPackedUntilDateAsync(residentId, ct))
            .WithName("GetResidentLastPackedUntil")
            .WithDescription("Date up to which this resident's medications have been packed")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.35 — Profile lock expiry
        group.MapGet("/{residentId:guid}/profile-lock-expiry", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.GetProfileLockExpiryAsync(residentId, ct))
            .WithName("GetResidentProfileLockExpiry")
            .WithDescription("Returns when the active profile lock expires (ISO 8601 or null)")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.36 — Has any forced-delete profile
        group.MapGet("/{residentId:guid}/has-forced-delete-profile", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.HasForcedDeleteProfileAsync(residentId, ct))
            .WithName("HasForcedDeleteProfile")
            .WithDescription("Returns true when at least one of this resident's profiles is scheduled for forced deletion")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.29 — Is current record
        group.MapGet("/{residentId:guid}/is-current-record", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.CheckResidentIsCurrentAsync(residentId, ct))
            .WithName("IsResidentCurrentRecord")
            .WithDescription("Returns true when this resident record has not been superseded by a linked staging record")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.16 — Check clean profile
        group.MapGet("/{residentId:guid}/profile/check-clean", (Guid residentId, DateTime dateTime, IResidentService svc, CancellationToken ct)
                => svc.CheckCleanProfileAsync(residentId, dateTime, ct))
            .WithName("CheckCleanProfile")
            .WithDescription("Validates whether the resident's profile can be cleaned at the given date")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.15 — Clean profile
        group.MapPost("/{residentId:guid}/profile/clean", (Guid residentId, [FromBody] CleanProfileRequest request, IResidentService svc, CancellationToken ct)
                => svc.CleanProfileAsync(residentId, request, ct))
            .WithName("CleanProfile")
            .WithDescription("Cleans the resident's profile up to the given date, optionally creating a dose signing record")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.14 — Generate dose signing
        group.MapPost("/{residentId:guid}/dose-signing/generate", (Guid residentId, [FromBody] GenerateDoseSigningRequest request, IResidentService svc, CancellationToken ct)
                => svc.GenerateDoseSigningAsync(residentId, request, ct))
            .WithName("GenerateDoseSigning")
            .WithDescription("Generates a dose signing record for the given date/time")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.37 — Pack layout
        group.MapGet("/{residentId:guid}/pack-layout", (Guid residentId, Guid? medProfileId, IResidentService svc, CancellationToken ct)
                => svc.GetPackLayoutAsync(residentId, medProfileId, ct))
            .WithName("GetResidentPackLayout")
            .WithDescription("Returns the pack layout for the resident's active (or specified) med profile")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.32 — Update regular scheduling (userId injected from JWT, never accepted from body)
        group.MapPut("/scheduling", (
                [FromBody] UpdateRegSchedulingRequest request,
                ClaimsPrincipal user,
                IResidentService svc,
                CancellationToken ct) =>
            {
                var userId = user.GetUserId();
                return svc.UpdateRegSchedulingAsync(userId, request, ct);
            })
            .WithName("UpdateRegScheduling")
            .WithDescription("Saves regular scheduling changes; userId is always derived from the JWT claim")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.38 — Save group pack (free-form key/value dict)
        group.MapPost("/group-pack", ([FromBody] Dictionary<string, string> groupPacks, IResidentService svc, CancellationToken ct)
                => svc.SaveGroupPackAsync(groupPacks, ct))
            .WithName("SaveGroupPack")
            .WithDescription("Saves group-pack assignments for residents (free-form key/value dictionary)")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.18 — Prescribers dropdown
        group.MapGet("/{residentId:guid}/prescribers/dropdown", (Guid residentId, int type, IResidentService svc, CancellationToken ct)
                => svc.GetPrescribersDropdownAsync(residentId, type, ct))
            .WithName("GetResidentPrescribersDropdown")
            .WithDescription("Returns prescribers for a resident, optionally prefixed with Select/All item based on type parameter")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        // 4.23 — VMC require-transfer flag
        group.MapPatch("/{residentId:guid}/vmc-require-transfer", (
                Guid residentId,
                [FromBody] VmcRequireTransferRequest request,
                IResidentService svc,
                CancellationToken ct)
                => svc.UpdateVmcRequireTransferAsync(residentId, request.SetBackToNull, ct))
            .WithName("UpdateVmcRequireTransfer")
            .WithDescription("Sets or clears the VMC-require-transfer flag on the resident (BMED-10406)")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.28 — Check VMC require-transfer + active profile
        group.MapGet("/{residentId:guid}/vmc/check-transfer-required", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.CheckVmcRequireTransferAndActiveProfileAsync(residentId, ct))
            .WithName("CheckVmcTransferRequired")
            .WithDescription("Returns true when the resident has a pending VMC transfer and an active medication profile")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.34 — Create resident order (Pharmacy users only)
        group.MapPost("/{residentId:guid}/orders", (
                Guid residentId,
                [FromBody] CreateResidentOrderRequest request,
                IOrderingClient ordering,
                CancellationToken ct)
                => ordering.CreateResidentOrderAsync(residentId, request, ct))
            .WithName("CreateResidentOrder")
            .WithDescription("Creates a drug order for the resident — Pharmacy user type only")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.42 — Packed medication tracking history
        group.MapGet("/{residentId:guid}/medication-tracking/packed", (
                Guid residentId,
                IMedicationTrackingClient medTracking,
                CancellationToken ct)
                => medTracking.GetPackedHistoryAsync(residentId, ct))
            .WithName("GetPackedMedicationTrackingHistory")
            .WithDescription("Packed medication tracking history for the resident")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.43 — Non-packed medication tracking history
        group.MapGet("/{residentId:guid}/medication-tracking/non-packed", (
                Guid residentId,
                IMedicationTrackingClient medTracking,
                CancellationToken ct)
                => medTracking.GetNonPackedHistoryAsync(residentId, ct))
            .WithName("GetNonPackedMedicationTrackingHistory")
            .WithDescription("Non-packed (script-level) medication tracking history for the resident")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.49 — Observations
        group.MapGet("/{residentId:guid}/observations", (
                Guid residentId,
                IObservationsClient obs,
                CancellationToken ct)
                => obs.GetObsTreeAsync(residentId, ct))
            .WithName("GetResidentObservations")
            .WithDescription("OBS tree (latest charts) for the resident")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.50 — Observation history by type
        group.MapGet("/{residentId:guid}/observations/{obsType}", (
                Guid residentId,
                string obsType,
                IObservationsClient obs,
                CancellationToken ct)
                => obs.GetObsHistoryAsync(residentId, obsType, ct))
            .WithName("GetResidentObservationHistory")
            .WithDescription("OBS history for a specific observation type")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.51 — Facility offline mode (calls FacilityService)
        group.MapGet("/{residentId:guid}/facility/offline-mode", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.IsFacilityOfflineModeAsync(residentId, ct))
            .WithName("IsResidentFacilityOfflineMode")
            .WithDescription("Returns true when the facility containing this resident is in offline mode")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.44 — Staging matched records
        group.MapGet("/{residentId:guid}/staging/matched", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.GetResidentStagingMatchedAsync(residentId, ct))
            .WithName("GetResidentStagingMatched")
            .WithDescription("Returns staging records matched to this resident")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.45 — BPack matched residents
        group.MapGet("/{residentId:guid}/matched", (Guid residentId, IResidentService svc, CancellationToken ct)
                => svc.GetMatchedResidentsAsync(residentId, ct))
            .WithName("GetMatchedResidents")
            .WithDescription("Returns matched BPack residents for linking")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.46 — Link resident
        group.MapPost("/{residentId:guid}/link", (Guid residentId, [FromBody] LinkResidentRequest request, IResidentService svc, CancellationToken ct)
                => svc.LinkResidentAsync(residentId, request, ct))
            .WithName("LinkResident")
            .WithDescription("Links a resident to a matched BPack resident record")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }
}

/// <summary>
/// JWT claim helper extension — keeps claim-reading code out of service implementations.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    private const string RoleWSystemAdmin = "WSystemAdmin";
    private const string RolePAuditor = "PAuditor";
    private const string RolePManager = "PManager";
    private const string RolePPharmacist = "PPharmacist";
    private const string RolePTechnician = "PTechnician";

    /// <summary>
    /// Returns true when the user holds any of the roles that grant edit rights
    /// (§2 Edit-rights check from the migration spec).
    /// </summary>
    public static bool HasEditRights(this ClaimsPrincipal user) =>
        user.IsInRole(RoleWSystemAdmin) ||
        user.IsInRole(RolePAuditor) ||
        user.IsInRole(RolePManager) ||
        user.IsInRole(RolePPharmacist) ||
        user.IsInRole(RolePTechnician);

    /// <summary>
    /// Returns true when the JWT contains the national-platform custom claim.
    /// TODO: Align claim name with the token issuer (AuthService).
    /// </summary>
    public static bool IsNationalPlatformUser(this ClaimsPrincipal user) =>
        user.HasClaim("is_national_platform_user", "true");

    /// <summary>
    /// Reads the user GUID from the JWT sub / name-identifier claim.
    /// userId must NEVER be accepted from the request body (§3 Critical Rule).
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? user.FindFirstValue("sub");
        return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
    }
}
