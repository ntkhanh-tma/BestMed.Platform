using BestMed.Common.Contracts;

namespace BestMed.UserService.Clients;

/// <summary>
/// Typed HTTP client for querying PrescriberService.
/// Communication pattern: Direct HTTP (synchronous) — used when UserService needs
/// to validate a PrescriberId or look up prescriber details as part of a live request.
/// Results are cached by <see cref="CachingPrescriberServiceClient"/>.
/// </summary>
public interface IPrescriberServiceClient
{
    /// <summary>Returns a single prescriber by ID, or null if not found.</summary>
    Task<PrescriberContract?> GetPrescriberByIdAsync(Guid prescriberId, CancellationToken cancellationToken = default);
}
