namespace BestMed.AuthenticateService.Models;

public sealed class SsoDiscoverRequest
{
    public required string UserName { get; init; }
}

public sealed class AgencyLoginBaseRequest
{
    public required string UserName { get; init; }
    public string? EmailPassword { get; init; }
    public string? Loc { get; init; }
    public bool IsExternalLogin { get; init; }
    public string? ExternalTenantId { get; init; }
}

public sealed class AgencyLoginRequest
{
    public required AgencyLoginBaseRequest Login { get; init; }
    public required AgencyUserInfo AgencyUser { get; init; }
    public bool IsWitnessUser { get; init; }
}

public sealed class VerifyPinRequest
{
    public required string UserName { get; init; }
    public required string Pin { get; init; }
    public string? Loc { get; init; }
    public bool RememberMe { get; init; }
}

public sealed class PinVerifyStandaloneRequest
{
    public required string UserName { get; init; }
    public required string Pin { get; init; }
}

public sealed class ChangePasswordRequest
{
    public required string NewPassword { get; init; }
    public required string ReTypePassword { get; init; }
    public string? Loc { get; init; }
}

public sealed class RequestPasswordResetRequest
{
    public required string EmailId { get; init; }
    public string? Loc { get; init; }
}

public sealed class ChangePasswordByTokenRequest
{
    public required string ResetCode { get; init; }
    public required string NewPassword { get; init; }
    public required string ReTypePassword { get; init; }
}

public sealed class ResetPasswordBySmsRequest
{
    public required string EmailId { get; init; }
    public required string SmsCode { get; init; }
    public required string NewPassword { get; init; }
    public required string ReTypePassword { get; init; }
    public required string MobileNumber { get; init; }
    public string? Loc { get; init; }
}

public sealed class ResetPasswordSmsSendRequest
{
    public required string EmailId { get; init; }
    public string? Loc { get; init; }
}

public sealed class RegisterDeviceRequest
{
    public required string Login { get; init; }
    public required string Pin { get; init; }
    public string? Loc { get; init; }
}

public sealed class SwitchOrganisationRequest
{
    public required Guid OrganisationId { get; init; }
}

public sealed class SwitchSectionRequest
{
    public required Guid SectionId { get; init; }
}

public sealed class SwitchToFacilityRequest
{
    public required Guid FacilityId { get; init; }
}

public sealed class NonBestmedPharmacistCheckRequest
{
    public required string LoginId { get; init; }
    public required string EmailPassword { get; init; }
    public string? Location { get; init; }
}

public sealed class ClientCredentialsTokenRequest
{
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public string? Scope { get; init; }
    public string? GrantType { get; init; }
}
