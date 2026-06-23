using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BestMed.AuthenticateService.Models;
using Microsoft.IdentityModel.Tokens;

namespace BestMed.AuthenticateService.Services;

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string IssueToken(
        UserBO user,
        UserIdentification identity,
        Guid? currentOrgId,
        string currentOrgName,
        string? hpioNumber,
        bool hasAllResidentAccess,
        (bool IsExpired, int DaysLeft) passwordExpiry,
        string areaName)
    {
        var claims = new List<Claim>
        {
            new(ClaimConstants.UserName, user.UserId),
            new(ClaimConstants.UserId, user.Id.ToString()),
            new(ClaimConstants.FirstName, user.FirstName),
            new(ClaimConstants.LastName, user.LastName),
            new(ClaimConstants.UserRole, user.RoleCode),
            new(ClaimConstants.UserType, user.Type),
            new(ClaimConstants.IsTermsAndConditionsAccepted, user.IsTermsAndConditionsAccepted.ToString()),
            new(ClaimConstants.IsInitialLogin, user.IsInitialLogin.ToString()),
            new(ClaimConstants.IsPasswordExpired, passwordExpiry.IsExpired.ToString()),
            new(ClaimConstants.PasswordDaysLeft, passwordExpiry.DaysLeft.ToString()),
            new(ClaimConstants.CurrentSection, user.LastSectionId.ToString()),
            new(ClaimConstants.OriginalId, user.OriginalId.ToString()),
            new(ClaimConstants.WebsiteType, "NationalPlatform"),
            new(ClaimConstants.UserMode, identity.Mode),
            new(ClaimConstants.HasAllResidentAccess, hasAllResidentAccess.ToString()),
            new(ClaimConstants.AreaName, areaName),
            new(ClaimConstants.IsExternalLogin, user.IsExternalLogin.ToString()),
            new(ClaimConstants.ExternalUserId, user.ExternalLoginId),
            new(ClaimConstants.IsBhsStaff, user.IsBHSStaff.ToString()),
            new(ClaimConstants.IsReadOnlyAccess, user.IsReadOnlyAccess.ToString()),
        };

        if (currentOrgId.HasValue)
        {
            claims.Add(new(ClaimConstants.CurrentOrganisationId, currentOrgId.Value.ToString()));
            claims.Add(new(ClaimConstants.CurrentOrganisationName, currentOrgName));
        }

        if (!string.IsNullOrEmpty(hpioNumber))
            claims.Add(new(ClaimConstants.HpioNumber, hpioNumber));

        if (user.ExternalTenantId.HasValue)
            claims.Add(new(ClaimConstants.ExternalTenantId, user.ExternalTenantId.Value.ToString()));

        foreach (var screen in identity.FeatureScreens)
            claims.Add(new(ClaimConstants.FeatureScreen, screen));

        foreach (var facility in identity.Facilities)
            claims.Add(new(ClaimConstants.FacilityId, facility.Id.ToString()));

        foreach (var pharmacy in identity.Pharmacies)
            claims.Add(new(ClaimConstants.PharmacyId, pharmacy.Id.ToString()));

        foreach (var warehouse in identity.WareHouses)
            claims.Add(new(ClaimConstants.WarehouseId, warehouse.Id.ToString()));

        foreach (var spf in identity.SupplyPharmacyFacilities)
            claims.Add(new(ClaimConstants.SupplyPharmacyFacilityId, spf.ToString()));

        foreach (var spfp in identity.SupplyPharmacyFacilityPharmacies)
            claims.Add(new(ClaimConstants.SupplyPharmacyFacilityPharmacyId, spfp.ToString()));

        var orgList = identity.Facilities.Cast<OrgInfo>()
            .Concat(identity.Pharmacies)
            .Concat(identity.WareHouses)
            .Select(o => new { id = o.Id, name = o.Name })
            .ToList();
        claims.Add(new(ClaimConstants.OrganisationList, JsonSerializer.Serialize(orgList, JsonOptions)));

        return CreateToken(claims);
    }

    public string UpdateClaims(ClaimsPrincipal current, IReadOnlyDictionary<string, string[]> updates)
    {
        var updateKeys = updates.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var claims = current.Claims
            .Where(c => !updateKeys.Contains(c.Type))
            .ToList();

        foreach (var (type, values) in updates)
            foreach (var value in values)
                claims.Add(new Claim(type, value));

        return CreateToken(claims);
    }

    private string CreateToken(IEnumerable<Claim> claims)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "BestMed";
        var audience = configuration["Jwt:Audience"] ?? "BestMed.Services";
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var expiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var m) ? m : 60;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
