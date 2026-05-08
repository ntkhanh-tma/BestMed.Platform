namespace BestMed.UserService.DTOs;

/// <summary>
/// Detailed user response including security and profile fields.
/// </summary>
public sealed record UserDetailDto
{
    public Guid Id { get; init; }
    public string? UserId { get; init; }
    public string? Email { get; init; }
    public string? NormalizedEmail { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PreferredName { get; init; }
    public string? Salutation { get; init; }
    public string? JobTitle { get; init; }
    public string? ContactNumber { get; init; }
    public string Type { get; init; } = null!;
    public string? Status { get; init; }
    public bool? IsActive { get; init; }
    public Guid RoleId { get; init; }
    public Guid? PrescriberId { get; init; }
    public bool IsExternalLogin { get; init; }
    public string? ExternalUserId { get; init; }
    public bool IsBESTmedLogin { get; init; }
    public bool IsBHSStaff { get; init; }
    public bool IsReadOnlyAccess { get; init; }
    public bool? IsProxyAccount { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool PhoneNumberConfirmed { get; init; }
    public bool TwoFactorEnabled { get; init; }
    public bool LockoutEnabled { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }
    public bool IsTermsAndConditionsAccepted { get; init; }
    public DateTime? TermsAndConditionsAcceptedDate { get; init; }
    public string? AHPRANumber { get; init; }
    public string? HPIINumber { get; init; }
    public string? HPIIStatus { get; init; }
    public string? IntegrationId { get; init; }
    public string? IntegrationSystem { get; init; }
    public string? UserQualifications { get; init; }
    public DateTime? LastLogin { get; init; }
    public DateTime? PasswordLastUpdated { get; init; }
    public DateTime? CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? LastUpdatedDate { get; init; }
    public Guid? LastUpdatedBy { get; init; }
}
