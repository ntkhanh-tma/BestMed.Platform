using BestMed.Common.Constants;
using BestMed.ResidentService.DTOs;
using BestMed.ResidentService.Entities;

namespace BestMed.ResidentService.Mapping;

public static class ResidentMappingExtensions
{
    // ── Resident ───────────────────────────────────────────────────────────────

    public static ResidentDto ToDto(this Resident r) => new()
    {
        Id = r.Id,
        FirstName = r.FirstName,
        LastName = r.LastName,
        DisplayName = r.DisplayName,
        FacilityId = r.FacilityId,
        SectionId = r.SectionId,
        Status = r.Status,
        FredCode = r.FredCode
    };

    public static ResidentDetailDto ToDetailDto(this Resident r) => new()
    {
        Id = r.Id,
        FirstName = r.FirstName,
        LastName = r.LastName,
        DisplayName = r.DisplayName,
        DateOfBirth = r.DateOfBirth,
        Gender = r.Gender,
        FacilityId = r.FacilityId,
        SectionId = r.SectionId,
        Status = r.Status,
        FredCode = r.FredCode,
        IsOtherSupplyPharmacy = r.IsOtherSupplyPharmacy,
        AlternativeSupplyPharmacyId = r.AlternativeSupplyPharmacyId,
        IsRestrictedByFacilityConfig = r.IsRestrictedByFacilityConfig,
        VMCRequireTransfer = r.VMCRequireTransfer,
        IHINumber = r.IHINumber,
        CreatedBy = r.CreatedBy,
        CreatedDate = r.CreatedDate,
        LastUpdatedBy = r.LastUpdatedBy,
        LastUpdatedDate = r.LastUpdatedDate
    };

    public static QuickSearchItemDto ToQuickSearchDto(this Resident r) => new()
    {
        ResidentId = r.Id,
        FirstName = r.FirstName,
        LastName = r.LastName,
        DisplayName = r.DisplayName,
        FacilityId = r.FacilityId,
        // FacilityName requires a join to the Facility table (cross-service).
        // TODO: Either denormalise FacilityName onto Resident or join via FacilityService.
        FacilityName = null,
        Status = r.Status
    };

    // ── MedProfile ─────────────────────────────────────────────────────────────

    public static MedProfileDto ToDto(this MedProfile p) => new()
    {
        Id = p.Id,
        ResidentId = p.ResidentId,
        Status = p.Status,
        IsActive = p.IsActive,
        IsForcedDelete = p.IsForcedDelete,
        LockStatus = p.LockStatus,
        LockedById = p.LockedById,
        LockedByName = p.LockedByName,
        LockedAt = p.LockedAt,
        LastChangedDate = p.LastChangedDate
    };

    // ── Filtering & Sorting ────────────────────────────────────────────────────

    public static IQueryable<Resident> ApplyFilters(this IQueryable<Resident> q, ResidentQueryParameters query)
    {
        if (!string.IsNullOrWhiteSpace(query.Status) &&
            !query.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            q = q.Where(r => r.Status == query.Status);

        if (query.FacilityId.HasValue)
            q = q.Where(r => r.FacilityId == query.FacilityId.Value);

        if (query.SectionId.HasValue)
            q = q.Where(r => r.SectionId == query.SectionId.Value);

        return q;
    }

    public static IQueryable<Resident> ApplySorting(this IQueryable<Resident> q, ResidentQueryParameters query)
    {
        var asc = SortDirection.IsAscending(query.SortDirection ?? "asc");
        return query.SortBy?.ToLowerInvariant() switch
        {
            "firstname" => asc ? q.OrderBy(r => r.FirstName) : q.OrderByDescending(r => r.FirstName),
            "status"    => asc ? q.OrderBy(r => r.Status)    : q.OrderByDescending(r => r.Status),
            _           => asc ? q.OrderBy(r => r.LastName).ThenBy(r => r.FirstName)
                               : q.OrderByDescending(r => r.LastName).ThenByDescending(r => r.FirstName)
        };
    }
}
