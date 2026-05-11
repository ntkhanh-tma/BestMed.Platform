using BestMed.Common.Constants;

namespace BestMed.WarehouseService.DTOs;

/// <summary>
/// Query parameters for searching/filtering warehouses with pagination.
/// </summary>
public sealed record WarehouseQueryParameters
{
    private int _pageSize = PaginationDefaults.DefaultPageSize;

    /// <summary>1-based page number.</summary>
    public int Page { get; init; } = PaginationDefaults.DefaultPage;

    /// <summary>Number of items per page (max 100).</summary>
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = PaginationDefaults.ClampPageSize(value);
    }

    /// <summary>Filter by warehouse name (contains, case-insensitive).</summary>
    public string? Name { get; init; }

    /// <summary>Filter by suburb (contains, case-insensitive).</summary>
    public string? Suburb { get; init; }

    /// <summary>Filter by state (exact match).</summary>
    public string? State { get; init; }

    /// <summary>Filter by multi-site flag.</summary>
    public bool? IsMultiSite { get; init; }

    /// <summary>Sort field: Name (default), Suburb, State.</summary>
    public string SortBy { get; init; } = "Name";

    /// <summary>Sort direction: asc (default) or desc.</summary>
    public string SortDirection { get; init; } = "asc";
}
