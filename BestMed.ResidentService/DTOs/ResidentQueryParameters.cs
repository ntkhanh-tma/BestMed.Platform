using BestMed.Common.Constants;

namespace BestMed.ResidentService.DTOs;

/// <summary>Query/filter parameters for GET /residents.</summary>
public sealed record ResidentQueryParameters
{
    private int _pageSize = PaginationDefaults.DefaultPageSize;

    /// <summary>1-based page number.</summary>
    public int Page { get; init; } = PaginationDefaults.DefaultPage;

    /// <summary>Number of items per page (capped at 100).</summary>
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = PaginationDefaults.ClampPageSize(value);
    }

    /// <summary>Filter by resident status (Active, Inactive, All, Discharged, etc.).</summary>
    public string? Status { get; init; }

    /// <summary>Filter by facility ID.</summary>
    public Guid? FacilityId { get; init; }

    /// <summary>Filter by section ID.</summary>
    public Guid? SectionId { get; init; }

    /// <summary>Filter by prescriber ID.</summary>
    public Guid? PrescriberId { get; init; }

    /// <summary>Filter by pharmacy ID.</summary>
    public Guid? PharmacyId { get; init; }

    /// <summary>Column to sort by. Default: LastName.</summary>
    public string? SortBy { get; init; }

    /// <summary>"asc" or "desc". Default: asc.</summary>
    public string? SortDirection { get; init; }
}
