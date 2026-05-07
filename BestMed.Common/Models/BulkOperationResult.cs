namespace BestMed.Common.Models;

/// <summary>
/// Standard result for bulk mutation operations across any service.
/// </summary>
public sealed record BulkOperationResult
{
    public int Succeeded { get; init; }
    public int NotFound { get; init; }
    public List<Guid> NotFoundIds { get; init; } = [];
}
