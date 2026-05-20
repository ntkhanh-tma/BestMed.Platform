using BestMed.Common.Constants;

namespace BestMed.FacilityService.DTOs;

/// <summary>
/// Query parameters for searching/filtering facilities with pagination.
/// </summary>
public sealed record FacilityQueryParameters
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

    /// <summary>Filter by facility name (contains, case-insensitive).</summary>
    public string? Name { get; init; }

    /// <summary>Filter by state (exact match).</summary>
    public string? State { get; init; }

    /// <summary>Filter by suburb (contains).</summary>
    public string? Suburb { get; init; }

    /// <summary>Filter by active status (int).</summary>
    public int? Active { get; init; }

    /// <summary>Filter by pharmacy ID.</summary>
    public Guid? PharmacyId { get; init; }

    /// <summary>Column to sort by. Default: name.</summary>
    public string? SortBy { get; init; }

    /// <summary>"asc" or "desc". Default: asc.</summary>
    public string? SortDirection { get; init; }
}
