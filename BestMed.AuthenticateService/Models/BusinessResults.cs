namespace BestMed.AuthenticateService.Models;

public sealed class VerifyUserResult
{
    public UserBO? User { get; init; }
    public string? Validation { get; init; }

    public bool IsSuccess => User is not null && string.IsNullOrEmpty(Validation);
}

public enum SsoProviderType
{
    None,
    Microsoft,
    Okta
}

public sealed class SsoProviderResult
{
    public string? Provider { get; init; }
    public SsoProviderType ProviderType { get; init; }
    public string? Log { get; init; }
    public string? ErrorCode { get; init; }
}

public enum SSOLoginLogType
{
    SSOLookup,
    SSOLogin
}

public sealed class AgencyUserInfo
{
    public string RegistrationNumber { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? DateOfBirth { get; init; }
}

public sealed class ResetPasswordResult
{
    public string? ResultCode { get; init; }
    public string? Result { get; init; }
    public string? Email { get; init; }

    public bool IsError => ResultCode == "Error";
}

public sealed class RegisterDeviceResult
{
    public string? ResultCode { get; init; }
    public string? Result { get; init; }
    public string? DeviceHash { get; init; }

    public bool IsError => ResultCode == "Error";
}

public sealed class SectionInfo
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class UserFacilityInfo
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool Selected { get; init; }
}
