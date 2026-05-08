using BestMed.UserService.DTOs;
using BestMed.UserService.Entities;

namespace BestMed.UserService.Mapping;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Email = entity.Email,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        PreferredName = entity.PreferredName,
        Salutation = entity.Salutation,
        JobTitle = entity.JobTitle,
        ContactNumber = entity.ContactNumber,
        Type = entity.Type,
        Status = entity.Status,
        IsActive = entity.IsActive,
        RoleId = entity.Role,
        PrescriberId = entity.PrescriberId,
        IsExternalLogin = entity.IsExternalLogin,
        ExternalUserId = entity.ExternalUserId,
        LastLogin = entity.LastLogin,
        CreatedDate = entity.CreatedDate,
        LastUpdatedDate = entity.LastUpdatedDate
    };

    public static UserDetailDto ToDetailDto(this User entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Email = entity.Email,
        NormalizedEmail = entity.NormalizedEmail,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        PreferredName = entity.PreferredName,
        Salutation = entity.Salutation,
        JobTitle = entity.JobTitle,
        ContactNumber = entity.ContactNumber,
        Type = entity.Type,
        Status = entity.Status,
        IsActive = entity.IsActive,
        RoleId = entity.Role,
        PrescriberId = entity.PrescriberId,
        IsExternalLogin = entity.IsExternalLogin,
        ExternalUserId = entity.ExternalUserId,
        IsBESTmedLogin = entity.IsBESTmedLogin,
        IsBHSStaff = entity.IsBHSStaff,
        IsReadOnlyAccess = entity.IsReadOnlyAccess,
        IsProxyAccount = entity.IsProxyAccount,
        EmailConfirmed = entity.EmailConfirmed,
        PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
        TwoFactorEnabled = entity.TwoFactorEnabled,
        LockoutEnabled = entity.LockoutEnabled,
        LockoutEnd = entity.LockoutEnd,
        IsTermsAndConditionsAccepted = entity.IsTermsAndConditionsAccepted,
        TermsAndConditionsAcceptedDate = entity.TermsAndConditionsAcceptedDate,
        AHPRANumber = entity.AHPRANumber,
        HPIINumber = entity.HPIINumber,
        HPIIStatus = entity.HPIIStatus,
        IntegrationId = entity.IntegrationId,
        IntegrationSystem = entity.IntegrationSystem,
        UserQualifications = entity.UserQualifications,
        LastLogin = entity.LastLogin,
        PasswordLastUpdated = entity.PasswordLastUpdated,
        CreatedDate = entity.CreatedDate,
        CreatedBy = entity.CreatedBy,
        LastUpdatedDate = entity.LastUpdatedDate,
        LastUpdatedBy = entity.LastUpdatedBy
    };
}
