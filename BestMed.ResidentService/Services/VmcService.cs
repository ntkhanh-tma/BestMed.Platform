using BestMed.ResidentService.DTOs;

namespace BestMed.ResidentService.Services;

/// <summary>
/// VMC (Virtual Med Count) operations.
/// TODO: All methods are stubs pending extraction of the VMC/Script domain into a
///       dedicated microservice (or a deliberate decision to own VMC data here).
///       When ready, inject the appropriate DbContext or HTTP client and implement.
/// </summary>
public sealed class VmcService(ILogger<VmcService> logger) : IVmcService
{
    public Task<IResult> GetResidentVmcAsync(Guid residentId, CancellationToken cancellationToken)
    {
        logger.LogWarning("GetResidentVmc called for {ResidentId} — VMC service not yet implemented", residentId);
        // TODO: Query VirtualMedCount records from the VMC domain store.
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public Task<IResult> GetVmcDetailsAsync(Guid residentId, Guid drugId, CancellationToken cancellationToken)
    {
        logger.LogWarning("GetVmcDetails called for resident {ResidentId} drug {DrugId} — VMC service not yet implemented", residentId, drugId);
        // TODO: Return single VirtualMedCountDto for the resident+drug pair.
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public Task<IResult> GetVmcTransactionReportAsync(Guid residentId, Guid drugId, CancellationToken cancellationToken)
    {
        logger.LogWarning("GetVmcTransactionReport called for resident {ResidentId} drug {DrugId} — VMC service not yet implemented", residentId, drugId);
        // TODO: Return IEnumerable<VMCTransactionReportBo>.
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public Task<IResult> GetVmcReportViewAsync(Guid? residentId, Guid? drugId, string path, Guid pharmacyId, CancellationToken cancellationToken)
    {
        logger.LogWarning("GetVmcReportView called — VMC service not yet implemented");
        // TODO: Resolve drug name via IDrugClient.GetDrugNameAsync then return report metadata.
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public Task<IResult> UpdateVmcValueAsync(Guid residentId, Guid drugId, UpdateVmcValueRequest request, CancellationToken cancellationToken)
    {
        logger.LogWarning("UpdateVmcValue called for resident {ResidentId} drug {DrugId} — VMC service not yet implemented", residentId, drugId);
        // TODO: Multi-step operation:
        //   1. GetVirtualMedCount(residentId, drugId)            — return empty VirtualMedCountDto + log error if not found
        //   2. UpdateVirtualMedCountWithTransaction(vmc, description, source="Manual Adjustment", reason)
        //   3. UpdateResidentVMCRequireTransferField(residentId, setBackToNull=true)  — BMED-10406
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public Task<IResult> BulkSaveVmcAsync(BulkVmcUpdateRequest request, CancellationToken cancellationToken)
    {
        logger.LogWarning("BulkSaveVmc called with {Count} items — VMC service not yet implemented", request.Items.Count);
        // TODO: Call UpdateVmcsWithTransactions(vmcs, source="Manual Adjustment") — returns bool.
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public Task<IResult> TransferVmcAsync(VmcTransferRequest request, CancellationToken cancellationToken)
    {
        logger.LogWarning("TransferVmc called — VMC service not yet implemented");
        // TODO: Call UpdateResidentVmcTransferOldResidentToNewResident(items, reason) — returns bool.
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public Task<IResult> GetVmcTransferCandidatesAsync(Guid residentId, CancellationToken cancellationToken)
    {
        logger.LogWarning("GetVmcTransferCandidates called for {ResidentId} — VMC service not yet implemented", residentId);
        // TODO: Return IList<VMCTransferOldResidentToNewResidentBo>.
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public Task<IResult> CheckVmcBalanceReviewAsync(Guid residentId, Guid? drugId, CancellationToken cancellationToken)
    {
        logger.LogWarning("CheckVmcBalanceReview called for {ResidentId} — VMC service not yet implemented", residentId);
        // TODO: Return CheckAndGetIfHasVmcBalanceReviewRequired result (bool + review data).
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }

    public Task<IResult> SaveVmcBalanceReviewAsync(Guid residentId, SaveVmcBalanceReviewRequest request, CancellationToken cancellationToken)
    {
        logger.LogWarning("SaveVmcBalanceReview called for {ResidentId} — VMC service not yet implemented", residentId);
        // TODO: Persist the review action and return bool.
        return Task.FromResult(Results.StatusCode(StatusCodes.Status501NotImplemented));
    }
}
