using BestMed.ResidentService.DTOs;

namespace BestMed.ResidentService.Services;

/// <summary>
/// Virtual Med Count (VMC) operations — maps to the legacy IScriptMaintenanceBusiness.
/// All methods are currently stubs pending implementation of the VMC/Script domain service.
/// TODO: When the VMC domain is extracted into its own microservice, replace the
///       stub body with an HTTP call (or promote VMC as a sub-domain of ResidentService).
/// </summary>
public interface IVmcService
{
    Task<IResult> GetResidentVmcAsync(Guid residentId, CancellationToken cancellationToken);
    Task<IResult> GetVmcDetailsAsync(Guid residentId, Guid drugId, CancellationToken cancellationToken);
    Task<IResult> GetVmcTransactionReportAsync(Guid residentId, Guid drugId, CancellationToken cancellationToken);
    Task<IResult> GetVmcReportViewAsync(Guid? residentId, Guid? drugId, string path, Guid pharmacyId, CancellationToken cancellationToken);
    Task<IResult> UpdateVmcValueAsync(Guid residentId, Guid drugId, UpdateVmcValueRequest request, CancellationToken cancellationToken);
    Task<IResult> BulkSaveVmcAsync(BulkVmcUpdateRequest request, CancellationToken cancellationToken);
    Task<IResult> TransferVmcAsync(VmcTransferRequest request, CancellationToken cancellationToken);
    Task<IResult> GetVmcTransferCandidatesAsync(Guid residentId, CancellationToken cancellationToken);
    Task<IResult> CheckVmcBalanceReviewAsync(Guid residentId, Guid? drugId, CancellationToken cancellationToken);
    Task<IResult> SaveVmcBalanceReviewAsync(Guid residentId, SaveVmcBalanceReviewRequest request, CancellationToken cancellationToken);
}
