using System.ComponentModel.DataAnnotations;

namespace BestMed.PrescriberService.DTOs;

/// <summary>
/// Request DTO for updating a prescriber.
/// </summary>
public sealed record UpdatePrescriberRequest
{
    [StringLength(100)]
    public string? PrescriberName { get; init; }

    [StringLength(20)]
    public string? PrescriberCode { get; init; }

    [StringLength(50)]
    public string? FirstName { get; init; }

    [StringLength(50)]
    public string? LastName { get; init; }

    [StringLength(50)]
    public string? PreferredName { get; init; }

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; init; }

    [StringLength(20)]
    public string? Phone { get; init; }

    [StringLength(20)]
    public string? MobileNumber { get; init; }

    [StringLength(20)]
    public string? Fax { get; init; }

    [StringLength(20)]
    public string? OutOfHours { get; init; }

    [StringLength(100)]
    public string? Address1 { get; init; }

    [StringLength(100)]
    public string? Address2 { get; init; }

    [StringLength(50)]
    public string? Suburb { get; init; }

    [StringLength(10)]
    public string? State { get; init; }

    [StringLength(4)]
    public string? Postcode { get; init; }

    [StringLength(50)]
    public string? Country { get; init; }

    [StringLength(13)]
    public string? AHPRANumber { get; init; }

    [StringLength(20)]
    public string? HPIINumber { get; init; }

    [StringLength(50)]
    public string? HPIIStatus { get; init; }

    [StringLength(20)]
    public string? LicenseNumber { get; init; }

    [StringLength(30)]
    public string? Qualification { get; init; }

    public bool? EnableMimsDrugInteractionChecking { get; init; }

    [StringLength(100)]
    public string? DefaultMimsSeverityLevel { get; init; }

    [StringLength(100)]
    public string? DefaultMimsDocumentationLevel { get; init; }

    public bool? IseRxUserAccessAgreementAccepted { get; init; }

    [StringLength(200)]
    public string? ERxUserAccessAgreementVersion { get; init; }

    [StringLength(200)]
    public string? ERxEntityId { get; init; }
}
