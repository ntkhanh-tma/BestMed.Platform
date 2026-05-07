namespace BestMed.Common.Constants;

/// <summary>
/// Standard sort directions used across query endpoints.
/// </summary>
public static class SortDirection
{
    public const string Ascending = "asc";
    public const string Descending = "desc";

    public static bool IsAscending(string direction) =>
        direction.Equals(Ascending, StringComparison.OrdinalIgnoreCase);
}
