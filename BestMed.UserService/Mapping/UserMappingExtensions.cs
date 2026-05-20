using BestMed.Common.Constants;
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

    public static void ApplyTo(this UpdateUserRequest request, User user)
    {
        if (request.Email is not null) user.Email = request.Email;
        if (request.FirstName is not null) user.FirstName = request.FirstName;
        if (request.LastName is not null) user.LastName = request.LastName;
        if (request.PreferredName is not null) user.PreferredName = request.PreferredName;
        if (request.Salutation is not null) user.Salutation = request.Salutation;
        if (request.JobTitle is not null) user.JobTitle = request.JobTitle;
        if (request.ContactNumber is not null) user.ContactNumber = request.ContactNumber;
        if (request.Status is not null) user.Status = request.Status;
        if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;
        if (request.RoleId.HasValue) user.Role = request.RoleId.Value;
        if (request.IsReadOnlyAccess.HasValue) user.IsReadOnlyAccess = request.IsReadOnlyAccess.Value;
        user.LastUpdatedDate = DateTime.UtcNow;
    }

    public static void ApplyTo(this BulkUpdateUserItem item, User user)
    {
        if (item.Email is not null) user.Email = item.Email;
        if (item.FirstName is not null) user.FirstName = item.FirstName;
        if (item.LastName is not null) user.LastName = item.LastName;
        if (item.PreferredName is not null) user.PreferredName = item.PreferredName;
        if (item.ContactNumber is not null) user.ContactNumber = item.ContactNumber;
        if (item.Status is not null) user.Status = item.Status;
        if (item.IsActive.HasValue) user.IsActive = item.IsActive.Value;
        if (item.RoleId.HasValue) user.Role = item.RoleId.Value;
        if (item.IsReadOnlyAccess.HasValue) user.IsReadOnlyAccess = item.IsReadOnlyAccess.Value;
        user.LastUpdatedDate = DateTime.UtcNow;
    }

    public static IQueryable<User> ApplyFilters(this IQueryable<User> queryable, UserQueryParameters query)
    {
        if (!string.IsNullOrWhiteSpace(query.Email))
            queryable = queryable.Where(u => u.Email != null && u.Email.Contains(query.Email));

        if (!string.IsNullOrWhiteSpace(query.FirstName))
            queryable = queryable.Where(u => u.FirstName != null && u.FirstName.Contains(query.FirstName));

        if (!string.IsNullOrWhiteSpace(query.LastName))
            queryable = queryable.Where(u => u.LastName != null && u.LastName.Contains(query.LastName));

        if (query.IsActive.HasValue)
            queryable = queryable.Where(u => u.IsActive == query.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(query.Type))
            queryable = queryable.Where(u => u.Type == query.Type);

        if (!string.IsNullOrWhiteSpace(query.Status))
            queryable = queryable.Where(u => u.Status == query.Status);

        if (query.RoleId.HasValue)
            queryable = queryable.Where(u => u.Role == query.RoleId.Value);

        return queryable;
    }

    public static IQueryable<User> ApplySorting(this IQueryable<User> queryable, UserQueryParameters query)
    {
        var asc = SortDirection.IsAscending(query.SortDirection);
        return query.SortBy?.ToLowerInvariant() switch
        {
            "email" => asc ? queryable.OrderBy(u => u.Email) : queryable.OrderByDescending(u => u.Email),
            "firstname" => asc ? queryable.OrderBy(u => u.FirstName) : queryable.OrderByDescending(u => u.FirstName),
            "lastname" => asc ? queryable.OrderBy(u => u.LastName) : queryable.OrderByDescending(u => u.LastName),
            _ => asc ? queryable.OrderBy(u => u.CreatedDate) : queryable.OrderByDescending(u => u.CreatedDate)
        };
    }
}
