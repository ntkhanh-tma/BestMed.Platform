using System.ComponentModel.DataAnnotations;

namespace BestMed.PharmacyService.DTOs;

/// <summary>
/// Request DTO for updating a pharmacy.
/// Only non-null fields are applied (PATCH semantics).
/// </summary>
public sealed record UpdatePharmacyRequest
{
    [StringLength(100)]
    public string? Name { get; init; }

    public Guid? WarehouseId { get; init; }

    public bool? Active { get; init; }

    public int? PharmacyType { get; init; }

    [StringLength(100)]
    public string? Address1 { get; init; }

    [StringLength(100)]
    public string? Address2 { get; init; }

    [StringLength(50)]
    public string? Suburb { get; init; }

    [StringLength(30)]
    public string? State { get; init; }

    [StringLength(4)]
    public string? PostCode { get; init; }

    [StringLength(30)]
    public string? Country { get; init; }

    [StringLength(50)]
    public string? ContactName { get; init; }

    [StringLength(20)]
    public string? Phone { get; init; }

    [StringLength(20)]
    public string? Fax { get; init; }

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; init; }

    [StringLength(20)]
    public string? OutOfHours { get; init; }

    [StringLength(11)]
    public string? ABN { get; init; }

    [StringLength(20)]
    public string? PharmacyApprovalNumber { get; init; }

    [StringLength(20)]
    public string? HPIONumber { get; init; }

    public bool? HasPackingFacility { get; init; }

    public bool? IsMultiSite { get; init; }

    public bool? EnableDashboard { get; init; }

    [StringLength(20)]
    public string? Tier { get; init; }
}
