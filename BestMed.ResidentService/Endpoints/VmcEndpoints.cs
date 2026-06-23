using BestMed.ResidentService.DTOs;
using BestMed.ResidentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestMed.ResidentService.Endpoints;

/// <summary>
/// Endpoints for VMC (Virtual Med Count) operations.
/// All service methods currently return 501 — see VmcService for TODO details.
/// Registered from ResidentEndpoints.MapResidentEndpoints.
/// </summary>
public static class VmcEndpoints
{
    public static IEndpointRouteBuilder MapVmcEndpoints(this IEndpointRouteBuilder routes)
    {
        // ── Resident-scoped VMC routes ────────────────────────────────────────

        var residentVmcGroup = routes.MapGroup("/residents/{residentId:guid}/vmc")
            .WithTags("VMC")
            .RequireAuthorization();

        // 4.22 — All VMCs for a resident
        residentVmcGroup.MapGet("/", (Guid residentId, IVmcService svc, CancellationToken ct)
                => svc.GetResidentVmcAsync(residentId, ct))
            .WithName("GetResidentVmc")
            .WithDescription("Returns all Virtual Med Count records for the resident")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.26 — Single drug VMC detail
        residentVmcGroup.MapGet("/{drugId:guid}", (Guid residentId, Guid drugId, IVmcService svc, CancellationToken ct)
                => svc.GetVmcDetailsAsync(residentId, drugId, ct))
            .WithName("GetVmcDetails")
            .WithDescription("Returns the VMC record for a specific resident–drug pair")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.31 — Manual VMC value update (has side-effect: clears VMCRequireTransfer flag — BMED-10406)
        residentVmcGroup.MapPatch("/{drugId:guid}/value", (
                Guid residentId,
                Guid drugId,
                [FromBody] UpdateVmcValueRequest request,
                IVmcService svc,
                CancellationToken ct)
                => svc.UpdateVmcValueAsync(residentId, drugId, request, ct))
            .WithName("UpdateVmcValue")
            .WithDescription("Manually adjusts the VMC for a resident–drug pair; always clears the VMCRequireTransfer flag after (BMED-10406)")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.20 — VMC transaction report for a drug
        residentVmcGroup.MapGet("/{drugId:guid}/transactions", (Guid residentId, Guid drugId, IVmcService svc, CancellationToken ct)
                => svc.GetVmcTransactionReportAsync(residentId, drugId, ct))
            .WithName("GetVmcTransactionReport")
            .WithDescription("Returns the VMC transaction audit trail for a resident–drug pair")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.27 — Balance review check
        residentVmcGroup.MapGet("/balance-review", (Guid residentId, Guid? drugId, IVmcService svc, CancellationToken ct)
                => svc.CheckVmcBalanceReviewAsync(residentId, drugId, ct))
            .WithName("CheckVmcBalanceReview")
            .WithDescription("Returns whether a VMC balance review is required for the resident, optionally scoped to a drug")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.30 — Save balance review action
        residentVmcGroup.MapPost("/balance-review", (
                Guid residentId,
                [FromBody] SaveVmcBalanceReviewRequest request,
                IVmcService svc,
                CancellationToken ct)
                => svc.SaveVmcBalanceReviewAsync(residentId, request, ct))
            .WithName("SaveVmcBalanceReview")
            .WithDescription("Saves the selected balance review action for the resident")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.25 — VMC transfer candidates
        residentVmcGroup.MapGet("/transfer-candidates", (Guid residentId, IVmcService svc, CancellationToken ct)
                => svc.GetVmcTransferCandidatesAsync(residentId, ct))
            .WithName("GetVmcTransferCandidates")
            .WithDescription("Returns drugs eligible for VMC transfer from this resident to another")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // 4.11 — VMC transaction report view metadata
        residentVmcGroup.MapGet("/report", (
                Guid residentId,
                Guid? drugId,
                string? path,
                Guid pharmacyId,
                IVmcService svc,
                CancellationToken ct)
                => svc.GetVmcReportViewAsync(residentId, drugId, path ?? string.Empty, pharmacyId, ct))
            .WithName("GetVmcReportView")
            .WithDescription("Returns metadata for the VMC transaction report view (drug name, etc.)")
            .RequireRateLimiting(Extensions.RateLimitLight);

        // ── Cross-resident VMC routes ─────────────────────────────────────────

        var vmcGroup = routes.MapGroup("/residents/vmc")
            .WithTags("VMC")
            .RequireAuthorization();

        // 4.21 — Bulk VMC update
        vmcGroup.MapPut("/bulk", ([FromBody] BulkVmcUpdateRequest request, IVmcService svc, CancellationToken ct)
                => svc.BulkSaveVmcAsync(request, ct))
            .WithName("BulkSaveVmc")
            .WithDescription("Bulk-updates VMC values with transaction audit records (source: Manual Adjustment)")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.24 — VMC transfer between residents
        vmcGroup.MapPost("/transfer", ([FromBody] VmcTransferRequest request, IVmcService svc, CancellationToken ct)
                => svc.TransferVmcAsync(request, ct))
            .WithName("TransferVmc")
            .WithDescription("Transfers VMC balances from one resident to another")
            .RequireRateLimiting(Extensions.RateLimitStandard);

        // 4.12 — VMC update view (returns view metadata only; data loaded lazily by the client)
        // Note: legacy route was GET ResidentVmcUpdate — here mapped as a metadata endpoint.
        vmcGroup.MapGet("/update-view", (Guid residentId, bool? isCurrentTab, bool? allowShowRequireTransferMessage,
                CancellationToken ct) =>
            {
                // No data fetch — just return the parameters the UI needs to initialise the view.
                return Results.Ok(new
                {
                    ResidentId = residentId,
                    IsCurrentTab = isCurrentTab ?? false,
                    AllowShowResidentVmcRequireTransferMessage = allowShowRequireTransferMessage ?? false
                });
            })
            .WithName("GetVmcUpdateView")
            .WithDescription("Returns view-initialisation metadata for the VMC update screen")
            .RequireRateLimiting(Extensions.RateLimitLight)
            .CacheOutput("short");

        return routes;
    }
}
