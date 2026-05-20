using BestMed.Common.Constants;
using BestMed.WarehouseService.DTOs;
using BestMed.WarehouseService.Entities;

namespace BestMed.WarehouseService.Mapping;

public static class WarehouseMappingExtensions
{
    public static WarehouseDto ToDto(this Warehouse entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
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
        ABN = entity.ABN,
        IsMultiSite = entity.IsMultiSite,
        RestrictPreferredBrand = entity.RestrictPreferredBrand,
        HasThirdPartyPacking = entity.HasThirdPartyPacking,
        StateTimeZoneId = entity.StateTimeZoneId,
        CreatedDate = entity.CreatedDate,
        LastUpdatedDate = entity.LastUpdatedDate
    };

    public static WarehouseDetailDto ToDetailDto(this Warehouse entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
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
        IPAddress = entity.IPAddress,
        IPDescription = entity.IPDescription,
        ABN = entity.ABN,
        XMLUserName = entity.XMLUserName,
        StateTimeZoneId = entity.StateTimeZoneId,
        CheckingMachineType = entity.CheckingMachineType,
        NewUserAttachmentId = entity.NewUserAttachmentId,
        GeoLocations = entity.GeoLocations,
        GeoRadius = entity.GeoRadius,
        SachetRobotTypeId = entity.SachetRobotTypeId,
        BlisterRobotTypeId = entity.BlisterRobotTypeId,
        YuyamaModelId = entity.YuyamaModelId,
        IsMultiSite = entity.IsMultiSite,
        RestrictPreferredBrand = entity.RestrictPreferredBrand,
        HasThirdPartyPacking = entity.HasThirdPartyPacking,
        PharmacyToInsert = entity.PharmacyToInsert,
        EnablePasswordAging = entity.EnablePasswordAging,
        PasswordAging = entity.PasswordAging,
        CreatedBy = entity.CreatedBy,
        CreatedDate = entity.CreatedDate,
        LastUpdatedBy = entity.LastUpdatedBy,
        LastUpdatedDate = entity.LastUpdatedDate,
        BankDetails = entity.BankDetails.Select(b => b.ToDto()).ToList(),
        Holidays = entity.Holidays.Select(h => h.ToDto()).ToList(),
        Robots = entity.Robots.Select(r => r.ToDto()).ToList()
    };

    public static WarehouseBankDetailDto ToDto(this WarehouseBankDetail entity) => new()
    {
        Id = entity.Id,
        BankName = entity.BankName,
        BSB = entity.BSB,
        AccountNumber = entity.AccountNumber
    };

    public static WarehouseHolidayDto ToDto(this WarehouseHoliday entity) => new()
    {
        Id = entity.Id,
        HolidayDate = entity.HolidayDate,
        HolidayName = entity.HolidayName,
        Description = entity.Description,
        State = entity.State
    };

    public static WarehouseRobotDto ToDto(this WarehouseRobot entity) => new()
    {
        Id = entity.Id,
        Type = entity.Type
    };

    public static void ApplyTo(this UpdateWarehouseRequest request, Warehouse warehouse)
    {
        if (request.Name is not null) warehouse.Name = request.Name;
        if (request.Address1 is not null) warehouse.Address1 = request.Address1;
        if (request.Address2 is not null) warehouse.Address2 = request.Address2;
        if (request.Suburb is not null) warehouse.Suburb = request.Suburb;
        if (request.State is not null) warehouse.State = request.State;
        if (request.PostCode is not null) warehouse.PostCode = request.PostCode;
        if (request.Country is not null) warehouse.Country = request.Country;
        if (request.ContactName is not null) warehouse.ContactName = request.ContactName;
        if (request.Phone is not null) warehouse.Phone = request.Phone;
        if (request.Fax is not null) warehouse.Fax = request.Fax;
        if (request.Email is not null) warehouse.Email = request.Email;
        if (request.IPDescription is not null) warehouse.IPDescription = request.IPDescription;
        if (request.ABN is not null) warehouse.ABN = request.ABN;
        if (request.StateTimeZoneId.HasValue) warehouse.StateTimeZoneId = request.StateTimeZoneId.Value;
        if (request.IsMultiSite.HasValue) warehouse.IsMultiSite = request.IsMultiSite.Value;
        if (request.RestrictPreferredBrand.HasValue) warehouse.RestrictPreferredBrand = request.RestrictPreferredBrand.Value;
        if (request.HasThirdPartyPacking.HasValue) warehouse.HasThirdPartyPacking = request.HasThirdPartyPacking.Value;
        if (request.PharmacyToInsert.HasValue) warehouse.PharmacyToInsert = request.PharmacyToInsert.Value;
        if (request.EnablePasswordAging.HasValue) warehouse.EnablePasswordAging = request.EnablePasswordAging.Value;
        if (request.PasswordAging.HasValue) warehouse.PasswordAging = request.PasswordAging.Value;
        warehouse.LastUpdatedDate = DateTime.UtcNow;
    }

    public static IQueryable<Warehouse> ApplyFilters(this IQueryable<Warehouse> queryable, WarehouseQueryParameters query)
    {
        if (!string.IsNullOrWhiteSpace(query.Name))
            queryable = queryable.Where(w => w.Name.Contains(query.Name));

        if (!string.IsNullOrWhiteSpace(query.Suburb))
            queryable = queryable.Where(w => w.Suburb != null && w.Suburb.Contains(query.Suburb));

        if (!string.IsNullOrWhiteSpace(query.State))
            queryable = queryable.Where(w => w.State == query.State);

        if (query.IsMultiSite.HasValue)
            queryable = queryable.Where(w => w.IsMultiSite == query.IsMultiSite.Value);

        return queryable;
    }

    public static IQueryable<Warehouse> ApplySorting(this IQueryable<Warehouse> queryable, WarehouseQueryParameters query)
    {
        var asc = SortDirection.IsAscending(query.SortDirection);
        return query.SortBy?.ToLowerInvariant() switch
        {
            "suburb" => asc ? queryable.OrderBy(w => w.Suburb) : queryable.OrderByDescending(w => w.Suburb),
            "state" => asc ? queryable.OrderBy(w => w.State) : queryable.OrderByDescending(w => w.State),
            _ => asc ? queryable.OrderBy(w => w.Name) : queryable.OrderByDescending(w => w.Name)
        };
    }
}
