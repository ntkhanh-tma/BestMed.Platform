using BestMed.ResidentService.DTOs;
using BestMed.ResidentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.ResidentService.Endpoints;

/// <summary>
/// Endpoints for /residents/med-profiles/{profileId} sub-resource.
/// Registered from ResidentEndpoints.MapResidentEndpoints.
/// </summary>
public static class MedProfileEndpoints
{
    public static IEndpointRouteBuilder MapMedProfileEndpoints(this IEndpointRouteBuilder routes)
    {
        // Resident-scoped med-profiles (paged list) — registered on the parent group in ResidentEndpoints.
        // The sub-resource group below handles operations on a single profileId.

        // 4.6 — Med profile list for a resident
        var residentGroup = routes.MapGroup("/residents/{residentId:guid}/med-profiles")
            .WithTags("MedProfiles")
            .RequireAuthorization();

        residentGroup.MapGet("/", (
                Guid residentId,
                bool? isActive,
                int? page,
                int? pageSize,
                IResidentService svc,
                CancellationToken ct)
                => svc.GetMedProfilesAsync(residentId, isActive ?? true, page ?? 1, pageSize ?? 25, ct))
            .WithName("GetResidentMedProfiles")
            .WithDescription("Paged list of medication profiles for a resident")
            .RequireRateLimiting(Extensions.RateLimitStandard)
            .CacheOutput("query");

        // Profile-level sub-resource group
        var profileGroup = routes.MapGroup("/residents/med-profiles/{profileId:guid}")
            .WithTags("MedProfiles")
            .RequireAuthorization();

        // 4.8 — Can be removed?
        profileGroup.MapGet("/can-remove", (Guid profileId, IResidentService svc, CancellationToken ct)
                => svc.CheckMedProfileCanBeRemovedAsync(profileId, ct))
            .WithName("CanRemoveMedProfile")
            .WithDescription("Returns true when the medication profile can be safely removed")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.9 — Delete med profile
        profileGroup.MapDelete("/", (Guid profileId, IResidentService svc, CancellationToken ct)
                => svc.RemoveMedProfileAsync(profileId, ct))
            .WithName("DeleteMedProfile")
            .WithDescription("Removes a medication profile")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.17 — Complete change verification
        profileGroup.MapPost("/complete-change-verification", (Guid profileId, IResidentService svc, CancellationToken ct)
                => svc.CompleteMedChangeVerificationAsync(profileId, ct))
            .WithName("CompleteMedChangeVerification")
            .WithDescription("Marks the medication profile as verified after a doctor-initiated change")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.39 — Is editable
        profileGroup.MapGet("/is-editable", (Guid profileId, string? status, string? lastUpdated, IResidentService svc, CancellationToken ct)
                => svc.IsProfileEditableAsync(profileId, status ?? string.Empty, lastUpdated ?? string.Empty, ct))
            .WithName("IsMedProfileEditable")
            .WithDescription("Returns an editability status indicator for the medication profile")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.40 — Check lock
        profileGroup.MapGet("/lock", (Guid profileId, string? lastUpdated, IResidentService svc, CancellationToken ct)
                => svc.CheckMedProfileLockAsync(profileId, lastUpdated ?? string.Empty, ct))
            .WithName("CheckMedProfileLock")
            .WithDescription("Returns the current lock state of the medication profile")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.41 — Invalid preferred brand PBS
        profileGroup.MapGet("/invalid-preferred-brand-pbs", (Guid profileId, IResidentService svc, CancellationToken ct)
                => svc.GetInvalidPreferredBrandPbsAsync(profileId, ct))
            .WithName("GetInvalidPreferredBrandPbs")
            .WithDescription("Returns preferred-brand PBS items on this profile that are no longer valid")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.10 — Remove pending chart profile (optimistic concurrency guard)
        var removePendingGroup = routes.MapGroup("/residents/med-profiles")
            .WithTags("MedProfiles")
            .RequireAuthorization();

        removePendingGroup.MapPost("/remove-pending", (
                [FromBody] RemovePendingChartProfileRequest request,
                IResidentService svc,
                CancellationToken ct)
                => svc.RemovePendingChartProfileAsync(request, ct))
            .WithName("RemovePendingChartProfile")
            .WithDescription("Removes a pending (chart) profile with optimistic concurrency protection")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        return routes;
    }
}
