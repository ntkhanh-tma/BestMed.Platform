using BestMed.Common.Constants;

namespace BestMed.PrescriberService.DTOs;

/// <summary>
/// Query parameters for searching/filtering prescribers with pagination.
/// </summary>
public sealed record PrescriberQueryParameters
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

    /// <summary>Filter by prescriber name (contains, case-insensitive).</summary>
    public string? PrescriberName { get; init; }

    /// <summary>Filter by prescriber code (exact match).</summary>
    public string? PrescriberCode { get; init; }

    /// <summary>Filter by first name (contains, case-insensitive).</summary>
    public string? FirstName { get; init; }

    /// <summary>Filter by last name (contains, case-insensitive).</summary>
    public string? LastName { get; init; }

    /// <summary>Filter by email (contains, case-insensitive).</summary>
    public string? Email { get; init; }

    /// <summary>Filter by AHPRA number (exact match).</summary>
    public string? AHPRANumber { get; init; }

    /// <summary>Sort field: PrescriberCode, FirstName, LastName, PrescriberName (default).</summary>
    public string SortBy { get; init; } = "PrescriberName";

    /// <summary>Sort direction: asc (default) or desc.</summary>
    public string SortDirection { get; init; } = "asc";
}
