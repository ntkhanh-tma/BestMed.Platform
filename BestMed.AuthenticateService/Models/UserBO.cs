namespace BestMed.AuthenticateService.Models;

public sealed class UserBO
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsTermsAndConditionsAccepted { get; set; }
    public bool IsInitialLogin { get; set; }
    public bool IsBESTmedLogin { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime? PasswordLastUpdated { get; set; }
    public Guid? LastFacilityId { get; set; }
    public Guid LastSectionId { get; set; }
    public bool LockToIP { get; set; }
    public bool IsExternalLogin { get; set; }
    public string ExternalLoginId { get; set; } = string.Empty;
    public Guid? ExternalTenantId { get; set; }
    public Guid OriginalId { get; set; }
    public bool IsBHSStaff { get; set; }
    public bool IsReadOnlyAccess { get; set; }
    public string AHPRANumber { get; set; } = string.Empty;
    public string UserQualifications { get; set; } = string.Empty;
}
