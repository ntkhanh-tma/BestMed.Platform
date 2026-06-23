namespace BestMed.ResidentService.DTOs;

/// <summary>
/// Virtual Med Count (VMC) record for a single resident–drug pair.
/// Maps to VirtualMedCountBo from legacy IScriptMaintenanceBusiness.
/// </summary>
public sealed record VirtualMedCountDto
{
    public Guid Id { get; init; }
    public Guid ResidentId { get; init; }
    public Guid DrugId { get; init; }
    public string? GenericCode { get; init; }
    public decimal Vmc { get; init; }
    public decimal PackCount { get; init; }
    public decimal? Soh { get; init; }
    public Guid CurrentScriptId { get; init; }
    public Guid InitialScriptId { get; init; }
    public bool Status { get; init; }
    public int OrderBy { get; init; }
    public decimal OriginalVmc { get; init; }
    public decimal? Quantity { get; init; }
    public string? Reason { get; init; }
}

/// <summary>Request body for PATCH /residents/{id}/vmc/{drugId}/value.</summary>
public sealed record UpdateVmcValueRequest
{
    public decimal NewValue { get; init; }
    public decimal? OldValue { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>Request body for PUT /residents/vmc/bulk.</summary>
public sealed record BulkVmcUpdateRequest
{
    public IReadOnlyList<VirtualMedCountDto> Items { get; init; } = [];
}

/// <summary>Single row in a VMC transfer operation.</summary>
public sealed record VmcTransferItemDto
{
    public Guid Id { get; init; }
    public Guid ResidentId { get; init; }
    public Guid DrugId { get; init; }
    public string? DrugName { get; init; }
    public decimal? OldRecordVmcBalance { get; init; }
    public decimal? NewRecordVmcBalance { get; init; }
    public decimal Vmc { get; init; }
    public bool CheckRow { get; init; }
    public bool Disabled { get; init; }
}

/// <summary>Request body for POST /residents/vmc/transfer.</summary>
public sealed record VmcTransferRequest
{
    public IReadOnlyList<VmcTransferItemDto> Items { get; init; } = [];
    public string Reason { get; init; } = string.Empty;
}

/// <summary>Request body for PATCH /residents/{id}/vmc-require-transfer.</summary>
public sealed record VmcRequireTransferRequest
{
    public bool SetBackToNull { get; init; }
}

/// <summary>Request body for POST /residents/{id}/vmc/balance-review.</summary>
public sealed record SaveVmcBalanceReviewRequest
{
    public string SelectedAction { get; init; } = string.Empty;
    public Guid? DrugId { get; init; }
    public string? Reason { get; init; }
}
