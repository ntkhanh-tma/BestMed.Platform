namespace BestMed.AuthenticateService.Models;

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public int ExpiresIn { get; init; } = 3600;
    public string UserType { get; init; } = string.Empty;
    public string RoleCode { get; init; } = string.Empty;
    public string AreaName { get; init; } = string.Empty;
    public bool IsInitialLogin { get; init; }
    public bool IsTermsAccepted { get; init; }
    public bool IsPasswordExpired { get; init; }
    public int PasswordDaysLeft { get; init; }
    public bool PasswordBreached { get; init; }
    public bool RequireAgencyStep { get; init; }
    public bool RequirePin { get; init; }
    public string? RoleCodeForAgency { get; init; }
}
