using BestMed.AuthenticateService.Models;

namespace BestMed.AuthenticateService.Services;

public interface IUserBusiness
{
    Task<VerifyUserResult> VerifyUserAsync(
        string userName, string password, string ipAddress, string userAgent,
        string? loc, string? deviceHash, bool allowAgency = false,
        CancellationToken ct = default);

    Task<VerifyUserResult> VerifyExternalUserAsync(
        string extUserId, string ipAddress, string userAgent, string loc,
        string? deviceHash, bool isRBAC, Guid facilityGroupId, string? extUserRole,
        CancellationToken ct = default);

    Task<SsoProviderResult> GetSSOProviderAsync(string userName, CancellationToken ct = default);

    Task CreateSSOLoginLogAsync(
        string userId, SSOLoginLogType logType, string? log,
        string ipAddress, bool success, CancellationToken ct = default);

    Task<bool> UserRBACEnableAsync(Guid facilityGroupId, CancellationToken ct = default);

    Task<bool> UserRoleTemplateValidAsync(Guid facilityGroupId, string roleName, CancellationToken ct = default);

    Task<List<Guid>> ValidateSSOFacilityIdAsync(
        Guid facilityGroupId, List<Guid> facilityIds, bool isAgencyNDISWorker,
        CancellationToken ct = default);

    Task<List<Guid>> ValidateSSOFacilityNameAsync(
        Guid facilityGroupId, List<string> facilityNames, bool isAgencyNDISWorker,
        CancellationToken ct = default);

    Task<UserBO> ApplyExternalUserRoleAsync(
        UserBO user, Guid facilityGroupId, string roleName, List<Guid> facilityIds,
        CancellationToken ct = default);

    Task<bool> ValidateUserGeolocationAsync(
        UserBO user, string ipAddress, List<Guid> facilityIds,
        CancellationToken ct = default);

    Task<UserBO?> AgencyUserLoginAsync(
        UserBO baseUser, AgencyUserInfo agencyUser, string ipAddress,
        string userAgent, string? loc, bool isWitness,
        CancellationToken ct = default);

    Task<UserIdentification> GetUserIdentificationAsync(UserBO user, CancellationToken ct = default);

    Task<bool> IsPINRequiredAsync(UserBO user, CancellationToken ct = default);

    Task<(bool IsValid, int FailedCount)> VerifyPinUserAsync(
        string userName, string pin, CancellationToken ct = default);

    Task UpdateLogForVerifyPinAsync(
        string userName, string pwd, string ip, string userAgent,
        string? loc, int failedCount, CancellationToken ct = default);

    Task<UserBO?> GetUserByUserIdAsync(string userName, CancellationToken ct = default);

    Task LogoutAsync(
        string loginId, Guid? realId, string ipAddress,
        string userAgent, string reason, CancellationToken ct = default);

    Task<bool> HasPasswordBreachedAsync(string password, CancellationToken ct = default);

    Task<bool> UpdateTermsAndConditionAsync(bool accepted, Guid userId, string ipAddress, CancellationToken ct = default);

    Task<bool> UpdateAgencyUserTermsAndConditionAsync(bool accepted, Guid userId, CancellationToken ct = default);

    Task<string?> ChangeInitialPasswordAsync(
        string newPassword, string ipAddress, string userAgent,
        string? loc, Guid userId, CancellationToken ct = default);

    Task<string?> ChangeExpiredPasswordAsync(
        string newPassword, string ipAddress, string userAgent,
        string? loc, Guid userId, CancellationToken ct = default);

    Task<bool> IsDoctorLoginWithValidMobileNumberAsync(string emailId, CancellationToken ct = default);

    Task<ResetPasswordResult> ResetPasswordAsync(
        string emailId, string ipAddress, string userAgent,
        Guid? sessionId, string? loc, string? deviceHash,
        CancellationToken ct = default);

    Task<string?> CheckExpiredResetPasswordTokenAsync(
        string token, string ipAddress, Guid? sessionId,
        string? deviceHash, CancellationToken ct = default);

    Task<string?> ChangePasswordByTokenAsync(
        string resetCode, string newPassword, string ipAddress,
        string userAgent, Guid? sessionId, CancellationToken ct = default);

    Task<string?> ResetPasswordBySmsAsync(
        string emailId, string smsCode, string newPassword,
        string ipAddress, string userAgent, string? loc,
        CancellationToken ct = default);

    Task<RegisterDeviceResult> RegisterDeviceAsync(
        string login, string pin, string ipAddress,
        string userAgent, Guid? sessionId, string? loc,
        CancellationToken ct = default);

    Task UpdateLastFacilityUsedAsync(Guid organisationId, string userType, CancellationToken ct = default);

    Task<List<SectionInfo>> GetFacilitySectionListAsync(Guid organisationId, CancellationToken ct = default);

    Task<List<string>> GetUserFeatureScreensAsync(Guid realId, Guid organisationId, CancellationToken ct = default);

    Task<bool> IsBESTtrackFacilityAsync(Guid organisationId, CancellationToken ct = default);

    Task<string?> GetHpioNumberAsync(Guid organisationId, string userType, CancellationToken ct = default);

    Task<bool> HasAllResidentAccessAsync(UserBO user, Guid facilityId, CancellationToken ct = default);

    Task UpdateSelectedSectionAsync(Guid sectionId, Guid userId, CancellationToken ct = default);

    Task<bool> IsFacilityHasS8BookAsync(Guid facilityId, CancellationToken ct = default);

    Task<List<UserFacilityInfo>> GetUserFacilityAsync(Guid realId, CancellationToken ct = default);

    Task<List<UserFacilityInfo>> GetUserFacilityForInPossessionReportAsync(Guid realId, CancellationToken ct = default);

    Task<object?> LookupAgencyUserAsync(
        string registrationNumber, string lastName,
        IEnumerable<string> roleCodes, CancellationToken ct = default);

    Task<object?> ValidateRegistrationNumberAsync(
        string registrationNumber, string roleCode, CancellationToken ct = default);

    Task<bool> IsShowNonBESTmedPharmacistFirstLoginConfirmAsync(
        string loginId, string password, string userAgent,
        string ipAddress, string location, CancellationToken ct = default);

    Task<string?> GetSupportHtmlContentAsync(CancellationToken ct = default);

    Task<object?> GetSupportInfoAsync(CancellationToken ct = default);

    Task<object?> GeneratePasswordResetVerificationCodeAsync(string loginId, CancellationToken ct = default);

    Task<List<Guid>> GetAllowOrgForIpAndGeoLocationAsync(
        List<Guid> orgIds, UserBO user, string ipAddress,
        string? geoLocation, string? allowedDeviceHash, CancellationToken ct = default);
}
