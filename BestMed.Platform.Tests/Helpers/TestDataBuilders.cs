using BestMed.FacilityService.DTOs;
using BestMed.PharmacyService.DTOs;
using BestMed.PrescriberService.DTOs;
using BestMed.RoleService.DTOs;
using BestMed.UserService.DTOs;
using BestMed.WarehouseService.DTOs;

namespace BestMed.Platform.Tests.Helpers;

/// <summary>
/// Centralised factory methods for test request/DTO objects.
/// Using a builder pattern keeps individual test files short and ensures
/// changes to DTO shapes are fixed in one place.
/// </summary>
internal static class TestDataBuilders
{
    // ── Shared ────────────────────────────────────────────────────────────────

    public static Guid NewId() => Guid.NewGuid();

    // ── User ──────────────────────────────────────────────────────────────────

    public static UpdateUserRequest AnUpdateUserRequest(
        string email = "user@example.com",
        string firstName = "Jane",
        string lastName = "Doe") =>
        new()
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true
        };

    public static UpdateUserStatusRequest AnUpdateUserStatusRequest(
        string status = "Active",
        bool isActive = true) =>
        new() { Status = status, IsActive = isActive };

    public static BulkUpdateUsersRequest ABulkUpdateUsersRequest(params Guid[] ids)
    {
        var targetIds = ids.Length > 0 ? ids : [NewId(), NewId()];
        return new()
        {
            Users = targetIds.Select(id => new BulkUpdateUserItem { Id = id, IsActive = true }).ToList()
        };
    }

    public static UserQueryParameters AUserQuery(int page = 1, int pageSize = 20) =>
        new() { Page = page, PageSize = pageSize };

    // ── Warehouse ─────────────────────────────────────────────────────────────

    public static CreateWarehouseRequest ACreateWarehouseRequest(string name = "Central Warehouse") =>
        new() { Name = name };

    public static UpdateWarehouseRequest AnUpdateWarehouseRequest(string name = "Updated Warehouse") =>
        new() { Name = name };

    public static UpdateWarehouseConfigRequest AnUpdateWarehouseConfigRequest() =>
        new() { ABN = "12345678901" };

    public static SaveWarehouseBankDetailRequest ASaveBankDetailRequest() =>
        new() { BankName = "Test Bank", BSB = "062000", AccountNumber = "12345678" };

    public static SaveWarehouseHolidayRequest ASaveHolidayRequest() =>
        new() { HolidayDate = DateTime.Today.AddDays(30), HolidayName = "Test Holiday" };

    public static WarehouseQueryParameters AWarehouseQuery(int page = 1, int pageSize = 20) =>
        new() { Page = page, PageSize = pageSize };

    public static WarehouseHolidayQueryParameters AHolidayQuery() => new();

    // ── Pharmacy ──────────────────────────────────────────────────────────────

    public static UpdatePharmacyRequest AnUpdatePharmacyRequest(string name = "Updated Pharmacy") =>
        new() { Name = name };

    public static PharmacyQueryParameters APharmacyQuery(int page = 1, int pageSize = 20) =>
        new() { Page = page, PageSize = pageSize };

    // ── Facility ──────────────────────────────────────────────────────────────

    public static UpdateFacilityRequest AnUpdateFacilityRequest(string name = "Updated Facility") =>
        new() { Name = name };

    public static FacilityQueryParameters AFacilityQuery(int page = 1, int pageSize = 20) =>
        new() { Page = page, PageSize = pageSize };

    // ── Role ──────────────────────────────────────────────────────────────────

    public static UpdateRoleRequest AnUpdateRoleRequest(string roleName = "Admin") =>
        new() { RoleName = roleName };

    public static RoleQueryParameters ARoleQuery(int page = 1, int pageSize = 20) =>
        new() { Page = page, PageSize = pageSize };

    // ── Prescriber ────────────────────────────────────────────────────────────

    public static UpdatePrescriberRequest AnUpdatePrescriberRequest(string name = "Dr. Smith") =>
        new() { PrescriberName = name };

    public static PrescriberQueryParameters APrescriberQuery(int page = 1, int pageSize = 20) =>
        new() { Page = page, PageSize = pageSize };
}
