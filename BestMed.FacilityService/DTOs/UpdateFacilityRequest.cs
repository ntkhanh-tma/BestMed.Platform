using System.ComponentModel.DataAnnotations;

namespace BestMed.FacilityService.DTOs;

/// <summary>
/// Request DTO for updating a facility.
/// Only non-null fields are applied (PATCH semantics).
/// </summary>
public sealed record UpdateFacilityRequest
{
    [StringLength(100)]
    public string? Name { get; init; }

    public Guid? PharmacyId { get; init; }

    public Guid? StateTimeZoneId { get; init; }

    public int? Active { get; init; }

    [StringLength(20)]
    public string? FacilityType { get; init; }

    [StringLength(100)]
    public string? ContactName { get; init; }

    [StringLength(20)]
    public string? FredCode { get; init; }

    [StringLength(10)]
    public string? RacId { get; init; }

    [StringLength(200)]
    public string? Address1 { get; init; }

    [StringLength(100)]
    public string? Address2 { get; init; }

    [StringLength(20)]
    public string? Suburb { get; init; }

    [StringLength(20)]
    public string? State { get; init; }

    [StringLength(10)]
    public string? PostCode { get; init; }

    [StringLength(20)]
    public string? Country { get; init; }

    [StringLength(15)]
    public string? Phone { get; init; }

    [StringLength(20)]
    public string? Fax { get; init; }

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; init; }

    [StringLength(11)]
    public string? ABN { get; init; }

    [StringLength(20)]
    public string? HPIONumber { get; init; }

    [StringLength(200)]
    public string? Region { get; init; }

    public bool? ActiveDirectoryEnabled { get; init; }

    [StringLength(20)]
    public string? SSOOption { get; init; }
}
