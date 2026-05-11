namespace BestMed.PrescriberService.DTOs;

/// <summary>
/// Response DTO for a prescriber.
/// </summary>
public sealed record PrescriberDto
{
    public Guid Id { get; init; }
    public string PrescriberName { get; init; } = null!;
    public string PrescriberCode { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PreferredName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? MobileNumber { get; init; }
    public string? Fax { get; init; }
    public string? OutOfHours { get; init; }
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? Suburb { get; init; }
    public string? State { get; init; }
    public string? Postcode { get; init; }
    public string? Country { get; init; }
    public string? AHPRANumber { get; init; }
    public string? HPIINumber { get; init; }
    public string? HPIIStatus { get; init; }
    public string? LicenseNumber { get; init; }
    public string? Qualification { get; init; }
    public bool PinAcknowledge { get; init; }
    public bool EnableMimsDrugInteractionChecking { get; init; }
    public bool? IseRxUserAccessAgreementAccepted { get; init; }
    public DateTime? ERxUserAccessAgreementAcceptedDate { get; init; }
    public string? ERxUserAccessAgreementVersion { get; init; }
    public string? ERxEntityId { get; init; }
}
