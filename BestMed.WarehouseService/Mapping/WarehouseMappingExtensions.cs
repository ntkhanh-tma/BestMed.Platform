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
}
