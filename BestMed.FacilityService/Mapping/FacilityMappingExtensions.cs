using BestMed.Common.Constants;
using BestMed.FacilityService.DTOs;
using BestMed.FacilityService.Entities;

namespace BestMed.FacilityService.Mapping;

public static class FacilityMappingExtensions
{
    public static FacilityDto ToDto(this Facility entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        PharmacyId = entity.PharmacyId,
        StateTimeZoneId = entity.StateTimeZoneId,
        Active = entity.Active,
        FacilityType = entity.FacilityType,
        ContactName = entity.ContactName,
        FredCode = entity.FredCode,
        RacId = entity.RacId,
        Address1 = entity.Address1,
        Address2 = entity.Address2,
        Suburb = entity.Suburb,
        State = entity.State,
        PostCode = entity.PostCode,
        Country = entity.Country,
        Phone = entity.Phone,
        Fax = entity.Fax,
        Email = entity.Email,
        ABN = entity.ABN,
        HPIONumber = entity.HPIONumber,
        Region = entity.Region,
        ActiveDirectoryEnabled = entity.ActiveDirectoryEnabled,
        SSOOption = entity.SSOOption
    };

    public static FacilityDetailDto ToDetailDto(this Facility entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        PharmacyId = entity.PharmacyId,
        StateTimeZoneId = entity.StateTimeZoneId,
        Active = entity.Active,
        FacilityType = entity.FacilityType,
        ContactName = entity.ContactName,
        FredCode = entity.FredCode,
        RacId = entity.RacId,
        Address1 = entity.Address1,
        Address2 = entity.Address2,
        Suburb = entity.Suburb,
        State = entity.State,
        PostCode = entity.PostCode,
        Country = entity.Country,
        Phone = entity.Phone,
        Fax = entity.Fax,
        Email = entity.Email,
        IPAddress = entity.IPAddress,
        IPDescription = entity.IPDescription,
        GeoLocations = entity.GeoLocations,
        GeoRadius = entity.GeoRadius,
        ABN = entity.ABN,
        HPIONumber = entity.HPIONumber,
        HPIOStatus = entity.HPIOStatus,
        EnablePasswordAging = entity.EnablePasswordAging,
        PasswordAging = entity.PasswordAging,
        ActiveDirectoryEnabled = entity.ActiveDirectoryEnabled,
        TenantId = entity.TenantId,
        Profit = entity.Profit,
        Region = entity.Region,
        Guidelines = entity.Guidelines,
        SSOOption = entity.SSOOption,
        EnableTrustedSSO = entity.EnableTrustedSSO,
        CreatedBy = entity.CreatedBy,
        CreatedDate = entity.CreatedDate,
        LastUpdatedBy = entity.LastUpdatedBy,
        LastUpdatedDate = entity.LastUpdatedDate,
        Sections = entity.Sections.Select(s => s.ToDto()).ToList()
    };

    public static SectionDto ToDto(this Section entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        FredCode = entity.FredCode,
        Phone = entity.Phone,
        Fax = entity.Fax,
        Email = entity.Email,
        IsActive = entity.IsActive
    };

    public static void ApplyTo(this UpdateFacilityRequest request, Facility facility)
    {
        if (request.Name is not null) facility.Name = request.Name;
        if (request.PharmacyId is not null) facility.PharmacyId = request.PharmacyId.Value;
        if (request.StateTimeZoneId is not null) facility.StateTimeZoneId = request.StateTimeZoneId;
        if (request.Active.HasValue) facility.Active = request.Active.Value;
        if (request.FacilityType is not null) facility.FacilityType = request.FacilityType;
        if (request.ContactName is not null) facility.ContactName = request.ContactName;
        if (request.FredCode is not null) facility.FredCode = request.FredCode;
        if (request.RacId is not null) facility.RacId = request.RacId;
        if (request.Address1 is not null) facility.Address1 = request.Address1;
        if (request.Address2 is not null) facility.Address2 = request.Address2;
        if (request.Suburb is not null) facility.Suburb = request.Suburb;
        if (request.State is not null) facility.State = request.State;
        if (request.PostCode is not null) facility.PostCode = request.PostCode;
        if (request.Country is not null) facility.Country = request.Country;
        if (request.Phone is not null) facility.Phone = request.Phone;
        if (request.Fax is not null) facility.Fax = request.Fax;
        if (request.Email is not null) facility.Email = request.Email;
        if (request.ABN is not null) facility.ABN = request.ABN;
        if (request.HPIONumber is not null) facility.HPIONumber = request.HPIONumber;
        if (request.Region is not null) facility.Region = request.Region;
        if (request.ActiveDirectoryEnabled.HasValue) facility.ActiveDirectoryEnabled = request.ActiveDirectoryEnabled.Value;
        if (request.SSOOption is not null) facility.SSOOption = request.SSOOption;
        facility.LastUpdatedDate = DateTime.UtcNow;
    }

    public static IQueryable<Facility> ApplyFilters(this IQueryable<Facility> queryable, FacilityQueryParameters query)
    {
        if (!string.IsNullOrWhiteSpace(query.Name))
            queryable = queryable.Where(f => f.Name.Contains(query.Name));

        if (!string.IsNullOrWhiteSpace(query.State))
            queryable = queryable.Where(f => f.State == query.State);

        if (!string.IsNullOrWhiteSpace(query.Suburb))
            queryable = queryable.Where(f => f.Suburb != null && f.Suburb.Contains(query.Suburb));

        if (query.Active.HasValue)
            queryable = queryable.Where(f => f.Active == query.Active.Value);

        if (query.PharmacyId.HasValue)
            queryable = queryable.Where(f => f.PharmacyId == query.PharmacyId.Value);

        return queryable;
    }

    public static IQueryable<Facility> ApplySorting(this IQueryable<Facility> queryable, FacilityQueryParameters query)
    {
        var asc = SortDirection.IsAscending(query.SortDirection);
        return query.SortBy?.ToLowerInvariant() switch
        {
            "state" => asc ? queryable.OrderBy(f => f.State) : queryable.OrderByDescending(f => f.State),
            "suburb" => asc ? queryable.OrderBy(f => f.Suburb) : queryable.OrderByDescending(f => f.Suburb),
            _ => asc ? queryable.OrderBy(f => f.Name) : queryable.OrderByDescending(f => f.Name)
        };
    }
}
