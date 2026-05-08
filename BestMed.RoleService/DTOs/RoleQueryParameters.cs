using BestMed.Common.Constants;

namespace BestMed.RoleService.DTOs;

/// <summary>
/// Query parameters for searching/filtering roles with pagination.
/// </summary>
public sealed record RoleQueryParameters
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

    /// <summary>Filter by role code (contains, case-insensitive).</summary>
    public string? RoleCode { get; init; }

    /// <summary>Filter by role name (contains, case-insensitive).</summary>
    public string? RoleName { get; init; }

    /// <summary>Filter by user type ID.</summary>
    public Guid? UserTypeId { get; init; }

    /// <summary>Sort field: RoleCode, RoleName (default).</summary>
    public string SortBy { get; init; } = "RoleName";

    /// <summary>Sort direction: asc (default) or desc.</summary>
    public string SortDirection { get; init; } = "asc";
}
