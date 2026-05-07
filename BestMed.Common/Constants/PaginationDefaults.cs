namespace BestMed.Common.Constants;

/// <summary>
/// Pagination defaults and limits used across all services.
/// </summary>
public static class PaginationDefaults
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    /// <summary>
    /// Clamps a requested page size to the allowed range [1, <see cref="MaxPageSize"/>].
    /// </summary>
    public static int ClampPageSize(int requested) =>
        requested > MaxPageSize ? MaxPageSize : requested < 1 ? DefaultPageSize : requested;
}
