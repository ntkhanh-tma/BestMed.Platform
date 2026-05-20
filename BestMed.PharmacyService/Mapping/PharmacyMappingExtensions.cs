using BestMed.Common.Constants;
using BestMed.PharmacyService.DTOs;
using BestMed.PharmacyService.Entities;

namespace BestMed.PharmacyService.Mapping;

public static class PharmacyMappingExtensions
{
    public static PharmacyDto ToDto(this Pharmacy entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        WarehouseId = entity.WarehouseId,
        Active = entity.Active,
        PharmacyType = entity.PharmacyType,
        Address1 = entity.Address1,
        Address2 = entity.Address2,
        Suburb = entity.Suburb,
        State = entity.State,
        PostCode = entity.PostCode,
        Country = entity.Country,
        ContactName = entity.ContactName,
        Phone = entity.Phone,
        Fax = entity.Fax,
        Email = entity.Email,
        OutOfHours = entity.OutOfHours,
        ABN = entity.ABN,
        PharmacyApprovalNumber = entity.PharmacyApprovalNumber,
        HPIONumber = entity.HPIONumber,
        HasPackingFacility = entity.HasPackingFacility,
        IsMultiSite = entity.IsMultiSite,
        EnableDashboard = entity.EnableDashboard,
        Tier = entity.Tier,
        ProgrammeJoinedDate = entity.ProgrammeJoinedDate
    };

    public static PharmacyDetailDto ToDetailDto(this Pharmacy entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        WarehouseId = entity.WarehouseId,
        StateTimeZoneId = entity.StateTimeZoneId,
        Active = entity.Active,
        PharmacyType = entity.PharmacyType,
        Address1 = entity.Address1,
        Address2 = entity.Address2,
        Suburb = entity.Suburb,
        State = entity.State,
        PostCode = entity.PostCode,
        Country = entity.Country,
        ShippingAddress1 = entity.ShippingAddress1,
        ShippingAddress2 = entity.ShippingAddress2,
        ShippingState = entity.ShippingState,
        ShippingSuburb = entity.ShippingSuburb,
        ShippingPostCode = entity.ShippingPostCode,
        ContactName = entity.ContactName,
        Phone = entity.Phone,
        Fax = entity.Fax,
        Email = entity.Email,
        OutOfHours = entity.OutOfHours,
        IPAddress = entity.IPAddress,
        IPDescription = entity.IPDescription,
        ABN = entity.ABN,
        PharmacyApprovalNumber = entity.PharmacyApprovalNumber,
        HPIONumber = entity.HPIONumber,
        HPIOStatus = entity.HPIOStatus,
        HasPackingFacility = entity.HasPackingFacility,
        S8DrugPackingAllowed = entity.S8DrugPackingAllowed,
        IsMultiSite = entity.IsMultiSite,
        EnablePasswordAging = entity.EnablePasswordAging,
        PasswordAging = entity.PasswordAging,
        GeoLocations = entity.GeoLocations,
        GeoRadius = entity.GeoRadius,
        EnableDashboard = entity.EnableDashboard,
        Tier = entity.Tier,
        ProgrammeJoinedDate = entity.ProgrammeJoinedDate,
        CreatedBy = entity.CreatedBy,
        CreatedDate = entity.CreatedDate,
        LastUpdatedBy = entity.LastUpdatedBy,
        LastUpdatedDate = entity.LastUpdatedDate,
        Facilities = entity.Facilities.Select(f => new PharmacyFacilityDto { Id = f.Id }).ToList()
    };

    public static void ApplyTo(this UpdatePharmacyRequest request, Pharmacy pharmacy)
    {
        if (request.Name is not null) pharmacy.Name = request.Name;
        if (request.WarehouseId is not null) pharmacy.WarehouseId = request.WarehouseId;
        if (request.Active.HasValue) pharmacy.Active = request.Active.Value;
        if (request.PharmacyType.HasValue) pharmacy.PharmacyType = request.PharmacyType.Value;
        if (request.Address1 is not null) pharmacy.Address1 = request.Address1;
        if (request.Address2 is not null) pharmacy.Address2 = request.Address2;
        if (request.Suburb is not null) pharmacy.Suburb = request.Suburb;
        if (request.State is not null) pharmacy.State = request.State;
        if (request.PostCode is not null) pharmacy.PostCode = request.PostCode;
        if (request.Country is not null) pharmacy.Country = request.Country;
        if (request.ContactName is not null) pharmacy.ContactName = request.ContactName;
        if (request.Phone is not null) pharmacy.Phone = request.Phone;
        if (request.Fax is not null) pharmacy.Fax = request.Fax;
        if (request.Email is not null) pharmacy.Email = request.Email;
        if (request.OutOfHours is not null) pharmacy.OutOfHours = request.OutOfHours;
        if (request.ABN is not null) pharmacy.ABN = request.ABN;
        if (request.PharmacyApprovalNumber is not null) pharmacy.PharmacyApprovalNumber = request.PharmacyApprovalNumber;
        if (request.HPIONumber is not null) pharmacy.HPIONumber = request.HPIONumber;
        if (request.HasPackingFacility.HasValue) pharmacy.HasPackingFacility = request.HasPackingFacility.Value;
        if (request.IsMultiSite.HasValue) pharmacy.IsMultiSite = request.IsMultiSite.Value;
        if (request.EnableDashboard.HasValue) pharmacy.EnableDashboard = request.EnableDashboard.Value;
        if (request.Tier is not null) pharmacy.Tier = request.Tier;
        pharmacy.LastUpdatedDate = DateTime.UtcNow;
    }

    public static IQueryable<Pharmacy> ApplyFilters(this IQueryable<Pharmacy> queryable, PharmacyQueryParameters query)
    {
        if (!string.IsNullOrWhiteSpace(query.Name))
            queryable = queryable.Where(p => p.Name.Contains(query.Name));

        if (!string.IsNullOrWhiteSpace(query.State))
            queryable = queryable.Where(p => p.State == query.State);

        if (!string.IsNullOrWhiteSpace(query.Suburb))
            queryable = queryable.Where(p => p.Suburb != null && p.Suburb.Contains(query.Suburb));

        if (query.Active.HasValue)
            queryable = queryable.Where(p => p.Active == query.Active.Value);

        if (query.WarehouseId.HasValue)
            queryable = queryable.Where(p => p.WarehouseId == query.WarehouseId.Value);

        return queryable;
    }

    public static IQueryable<Pharmacy> ApplySorting(this IQueryable<Pharmacy> queryable, PharmacyQueryParameters query)
    {
        var asc = SortDirection.IsAscending(query.SortDirection);
        return query.SortBy?.ToLowerInvariant() switch
        {
            "state" => asc ? queryable.OrderBy(p => p.State) : queryable.OrderByDescending(p => p.State),
            "suburb" => asc ? queryable.OrderBy(p => p.Suburb) : queryable.OrderByDescending(p => p.Suburb),
            _ => asc ? queryable.OrderBy(p => p.Name) : queryable.OrderByDescending(p => p.Name)
        };
    }
}
