using BestMed.Common.Constants;

namespace BestMed.UserService.DTOs;

/// <summary>
/// Query parameters for searching/filtering users with pagination.
/// </summary>
public sealed record UserQueryParameters
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

    /// <summary>Filter by email (contains, case-insensitive).</summary>
    public string? Email { get; init; }

    /// <summary>Filter by first name (contains, case-insensitive).</summary>
    public string? FirstName { get; init; }

    /// <summary>Filter by last name (contains, case-insensitive).</summary>
    public string? LastName { get; init; }

    /// <summary>Filter by active status.</summary>
    public bool? IsActive { get; init; }

    /// <summary>Filter by user type.</summary>
    public string? Type { get; init; }

    /// <summary>Filter by status.</summary>
    public string? Status { get; init; }

    /// <summary>Filter by role ID.</summary>
    public Guid? RoleId { get; init; }

    /// <summary>Sort field: Email, FirstName, LastName, CreatedDate (default).</summary>
    public string SortBy { get; init; } = "CreatedDate";

    /// <summary>Sort direction: asc or desc (default).</summary>
    public string SortDirection { get; init; } = "desc";
}
