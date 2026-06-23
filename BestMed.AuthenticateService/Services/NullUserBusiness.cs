using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

/// <summary>
/// Stub implementation of IUserBusiness.
/// Returns 501 Not Implemented for all operations until the backend user business
/// service is built and wired up. Replace with a real HTTP client or EF implementation.
/// </summary>
internal sealed class NullUserBusiness : IUserBusiness
{
    private static readonly Task<VerifyUserResult> NotImplementedVerify =
        Task.FromResult(new VerifyUserResult { Validation = "NotImplemented" });

    private static readonly Task<SsoProviderResult> NotImplementedSso =
        Task.FromResult(new SsoProviderResult { ProviderType = SsoProviderType.None, ErrorCode = "NotImplemented" });

    public Task<VerifyUserResult> VerifyUserAsync(string userName, string password, string ipAddress, string userAgent, string? loc, string? deviceHash, bool allowAgency = false, CancellationToken ct = default)
        => NotImplementedVerify;

    public Task<VerifyUserResult> VerifyExternalUserAsync(string extUserId, string ipAddress, string userAgent, string loc, string? deviceHash, bool isRBAC, Guid facilityGroupId, string? extUserRole, CancellationToken ct = default)
        => NotImplementedVerify;

    public Task<SsoProviderResult> GetSSOProviderAsync(string userName, CancellationToken ct = default)
        => NotImplementedSso;

    public Task CreateSSOLoginLogAsync(string userId, SSOLoginLogType logType, string? log, string ipAddress, bool success, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task<bool> UserRBACEnableAsync(Guid facilityGroupId, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<bool> UserRoleTemplateValidAsync(Guid facilityGroupId, string roleName, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<List<Guid>> ValidateSSOFacilityIdAsync(Guid facilityGroupId, List<Guid> facilityIds, bool isAgencyNDISWorker, CancellationToken ct = default)
        => Task.FromResult<List<Guid>>([]);

    public Task<List<Guid>> ValidateSSOFacilityNameAsync(Guid facilityGroupId, List<string> facilityNames, bool isAgencyNDISWorker, CancellationToken ct = default)
        => Task.FromResult<List<Guid>>([]);

    public Task<UserBO> ApplyExternalUserRoleAsync(UserBO user, Guid facilityGroupId, string roleName, List<Guid> facilityIds, CancellationToken ct = default)
        => Task.FromResult(user);

    public Task<bool> ValidateUserGeolocationAsync(UserBO user, string ipAddress, List<Guid> facilityIds, CancellationToken ct = default)
        => Task.FromResult(true);

    public Task<UserBO?> AgencyUserLoginAsync(UserBO baseUser, AgencyUserInfo agencyUser, string ipAddress, string userAgent, string? loc, bool isWitness, CancellationToken ct = default)
        => Task.FromResult<UserBO?>(null);

    public Task<UserIdentification> GetUserIdentificationAsync(UserBO user, CancellationToken ct = default)
        => Task.FromResult(new UserIdentification());

    public Task<bool> IsPINRequiredAsync(UserBO user, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<(bool IsValid, int FailedCount)> VerifyPinUserAsync(string userName, string pin, CancellationToken ct = default)
        => Task.FromResult((false, 0));

    public Task UpdateLogForVerifyPinAsync(string userName, string pwd, string ip, string userAgent, string? loc, int failedCount, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task<UserBO?> GetUserByUserIdAsync(string userName, CancellationToken ct = default)
        => Task.FromResult<UserBO?>(null);

    public Task LogoutAsync(string loginId, Guid? realId, string ipAddress, string userAgent, string reason, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task<bool> HasPasswordBreachedAsync(string password, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<bool> UpdateTermsAndConditionAsync(bool accepted, Guid userId, string ipAddress, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<bool> UpdateAgencyUserTermsAndConditionAsync(bool accepted, Guid userId, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<string?> ChangeInitialPasswordAsync(string newPassword, string ipAddress, string userAgent, string? loc, Guid userId, CancellationToken ct = default)
        => Task.FromResult<string?>("NotImplemented");

    public Task<string?> ChangeExpiredPasswordAsync(string newPassword, string ipAddress, string userAgent, string? loc, Guid userId, CancellationToken ct = default)
        => Task.FromResult<string?>("NotImplemented");

    public Task<bool> IsDoctorLoginWithValidMobileNumberAsync(string emailId, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<ResetPasswordResult> ResetPasswordAsync(string emailId, string ipAddress, string userAgent, Guid? sessionId, string? loc, string? deviceHash, CancellationToken ct = default)
        => Task.FromResult(new ResetPasswordResult { ResultCode = "Error", Result = "NotImplemented" });

    public Task<string?> CheckExpiredResetPasswordTokenAsync(string token, string ipAddress, Guid? sessionId, string? deviceHash, CancellationToken ct = default)
        => Task.FromResult<string?>("NotImplemented");

    public Task<string?> ChangePasswordByTokenAsync(string resetCode, string newPassword, string ipAddress, string userAgent, Guid? sessionId, CancellationToken ct = default)
        => Task.FromResult<string?>("NotImplemented");

    public Task<string?> ResetPasswordBySmsAsync(string emailId, string smsCode, string newPassword, string ipAddress, string userAgent, string? loc, CancellationToken ct = default)
        => Task.FromResult<string?>("NotImplemented");

    public Task<RegisterDeviceResult> RegisterDeviceAsync(string login, string pin, string ipAddress, string userAgent, Guid? sessionId, string? loc, CancellationToken ct = default)
        => Task.FromResult(new RegisterDeviceResult { ResultCode = "Error", Result = "NotImplemented" });

    public Task UpdateLastFacilityUsedAsync(Guid organisationId, string userType, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task<List<SectionInfo>> GetFacilitySectionListAsync(Guid organisationId, CancellationToken ct = default)
        => Task.FromResult<List<SectionInfo>>([]);

    public Task<List<string>> GetUserFeatureScreensAsync(Guid realId, Guid organisationId, CancellationToken ct = default)
        => Task.FromResult<List<string>>([]);

    public Task<bool> IsBESTtrackFacilityAsync(Guid organisationId, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<string?> GetHpioNumberAsync(Guid organisationId, string userType, CancellationToken ct = default)
        => Task.FromResult<string?>(null);

    public Task<bool> HasAllResidentAccessAsync(UserBO user, Guid facilityId, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task UpdateSelectedSectionAsync(Guid sectionId, Guid userId, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task<bool> IsFacilityHasS8BookAsync(Guid facilityId, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<List<UserFacilityInfo>> GetUserFacilityAsync(Guid realId, CancellationToken ct = default)
        => Task.FromResult<List<UserFacilityInfo>>([]);

    public Task<List<UserFacilityInfo>> GetUserFacilityForInPossessionReportAsync(Guid realId, CancellationToken ct = default)
        => Task.FromResult<List<UserFacilityInfo>>([]);

    public Task<object?> LookupAgencyUserAsync(string registrationNumber, string lastName, IEnumerable<string> roleCodes, CancellationToken ct = default)
        => Task.FromResult<object?>(null);

    public Task<object?> ValidateRegistrationNumberAsync(string registrationNumber, string roleCode, CancellationToken ct = default)
        => Task.FromResult<object?>(null);

    public Task<bool> IsShowNonBESTmedPharmacistFirstLoginConfirmAsync(string loginId, string password, string userAgent, string ipAddress, string location, CancellationToken ct = default)
        => Task.FromResult(false);

    public Task<string?> GetSupportHtmlContentAsync(CancellationToken ct = default)
        => Task.FromResult<string?>(null);

    public Task<object?> GetSupportInfoAsync(CancellationToken ct = default)
        => Task.FromResult<object?>(null);

    public Task<object?> GeneratePasswordResetVerificationCodeAsync(string loginId, CancellationToken ct = default)
        => Task.FromResult<object?>(null);

    public Task<List<Guid>> GetAllowOrgForIpAndGeoLocationAsync(List<Guid> orgIds, UserBO user, string ipAddress, string? geoLocation, string? allowedDeviceHash, CancellationToken ct = default)
        => Task.FromResult(orgIds);
}
