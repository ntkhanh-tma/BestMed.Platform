using System.Security.Claims;
using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

public interface IJwtTokenService
{
    string IssueToken(
        UserBO user,
        UserIdentification identity,
        Guid? currentOrgId,
        string currentOrgName,
        string? hpioNumber,
        bool hasAllResidentAccess,
        (bool IsExpired, int DaysLeft) passwordExpiry,
        string areaName);

    string UpdateClaims(ClaimsPrincipal current, IReadOnlyDictionary<string, string[]> updates);
}
