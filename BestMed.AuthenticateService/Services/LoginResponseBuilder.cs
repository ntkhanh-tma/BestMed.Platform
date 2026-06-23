using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

/// <summary>
/// Shared helper for building a <see cref="LoginResponse"/> from a verified user.
/// Encapsulates area routing (§6), password expiry (§7), HPIO, and resident-access resolution
/// so that AuthService, SsoService and PasswordService do not duplicate this logic.
/// </summary>
internal sealed class LoginResponseBuilder(IUserBusiness userBusiness, IJwtTokenService jwtService)
{
    internal async Task<(LoginResponse Response, string Token)> BuildAsync(
        UserBO user,
        string ipAddress,
        string? loc,
        string? deviceHash = null,
        bool checkPasswordBreached = false,
        string? password = null,
        CancellationToken ct = default)
    {
        var identity = await userBusiness.GetUserIdentificationAsync(user, ct);

        if (user.LockToIP)
        {
            var allOrgIds = identity.Facilities.Select(f => f.Id)
                .Concat(identity.Pharmacies.Select(p => p.Id))
                .Concat(identity.WareHouses.Select(w => w.Id))
                .ToList();

            var allowed = await userBusiness.GetAllowOrgForIpAndGeoLocationAsync(
                allOrgIds, user, ipAddress, loc, deviceHash, ct);

            var allowedSet = allowed.ToHashSet();
            identity.Facilities = identity.Facilities.Where(f => allowedSet.Contains(f.Id)).ToList();
            identity.Pharmacies = identity.Pharmacies.Where(p => allowedSet.Contains(p.Id)).ToList();
            identity.WareHouses = identity.WareHouses.Where(w => allowedSet.Contains(w.Id)).ToList();
        }

        var (currentOrgId, currentOrgName) = ResolveCurrentOrg(user, identity);
        var areaName = ComputeAreaName(user, identity, currentOrgId);
        var passwordExpiry = ComputePasswordExpiry(user, identity.Facilities, identity.Pharmacies, identity.WareHouses);

        var hpioNumber = currentOrgId.HasValue
            ? await userBusiness.GetHpioNumberAsync(currentOrgId.Value, user.Type, ct)
            : null;

        var hasAllResidentAccess = currentOrgId.HasValue
            && await userBusiness.HasAllResidentAccessAsync(user, currentOrgId.Value, ct);

        var passwordBreached = false;
        if (checkPasswordBreached && password is not null
            && (user.RoleCode == UserRoleCodes.FDoctor || user.RoleCode == UserRoleCodes.FDietician)
            && user.Type == UserTypes.Facility)
        {
            passwordBreached = await userBusiness.HasPasswordBreachedAsync(password, ct);
        }

        var token = jwtService.IssueToken(
            user, identity, currentOrgId, currentOrgName,
            hpioNumber, hasAllResidentAccess, passwordExpiry, areaName);

        var response = new LoginResponse
        {
            AccessToken = token,
            ExpiresIn = 3600,
            UserType = user.Type,
            RoleCode = user.RoleCode,
            AreaName = areaName,
            IsInitialLogin = user.IsInitialLogin,
            IsTermsAccepted = user.IsTermsAndConditionsAccepted,
            IsPasswordExpired = passwordExpiry.IsExpired,
            PasswordDaysLeft = passwordExpiry.DaysLeft,
            PasswordBreached = passwordBreached,
        };

        return (response, token);
    }

    internal static string ComputeAreaName(UserBO user, UserIdentification identity, Guid? currentOrgId)
        => user.Type switch
        {
            UserTypes.Pharmacy or UserTypes.Warehouse => AreaNames.BESTpackV2,
            UserTypes.AHService => AreaNames.BESTservice,
            UserTypes.Facility => ComputeFacilityAreaName(user, identity, currentOrgId),
            _ => AreaNames.BESTdose
        };

    internal static (bool IsExpired, int DaysLeft) ComputePasswordExpiry(
        UserBO user, List<OrgInfo> facilities, List<OrgInfo> pharmacies, List<OrgInfo> warehouses)
    {
        if (user.IsExternalLogin) return (false, int.MaxValue);
        if (UserRoleCodes.PasswordExpiryExcludeRoles.Contains(user.RoleCode)) return (false, int.MaxValue);
        if (user.PasswordLastUpdated is null) return (false, int.MaxValue);

        var minAging = facilities.Concat(pharmacies).Concat(warehouses)
            .Where(o => o.EnablePasswordAging && o.PasswordAging.HasValue)
            .Select(o => o.PasswordAging!.Value)
            .DefaultIfEmpty(0)
            .Min();

        if (minAging <= 0) return (false, int.MaxValue);

        var expiredAt = user.PasswordLastUpdated.Value.Date.AddMonths(minAging);
        var daysLeft = (int)(expiredAt - DateTime.UtcNow.Date).TotalDays;

        return (expiredAt < DateTime.UtcNow.Date, daysLeft);
    }

    internal static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 0) return email;

        var local = email[..atIndex];
        var showChars = local.Length > 3 ? 3 : 1;
        return local[..showChars] + "*******" + email[atIndex..];
    }

    private static (Guid? Id, string Name) ResolveCurrentOrg(UserBO user, UserIdentification identity)
    {
        if (user.LastFacilityId.HasValue && user.Type == UserTypes.Facility)
        {
            var last = identity.Facilities.FirstOrDefault(f => f.Id == user.LastFacilityId.Value);
            if (last is not null) return (last.Id, last.Name);
        }

        var first = identity.Facilities.FirstOrDefault()
                    ?? identity.Pharmacies.FirstOrDefault()
                    ?? identity.WareHouses.FirstOrDefault();

        return first is not null ? (first.Id, first.Name) : (null, string.Empty);
    }

    private static string ComputeFacilityAreaName(UserBO user, UserIdentification identity, Guid? currentOrgId)
    {
        if (user.RoleCode == UserRoleCodes.FDoctor || user.RoleCode == UserRoleCodes.FDietician)
            return AreaNames.BESTdoctor;

        var facility = currentOrgId.HasValue
            ? identity.Facilities.FirstOrDefault(f => f.Id == currentOrgId.Value)
            : null;

        if (user.RoleCode == UserRoleCodes.FHealthDepartmentInspector && facility?.State == "WA")
            return AreaNames.BESTtrack;

        if (facility?.FacilityType == FacilityTypes.BESTtrack)
            return AreaNames.BESTtrack;

        return AreaNames.BESTdose;
    }
}
