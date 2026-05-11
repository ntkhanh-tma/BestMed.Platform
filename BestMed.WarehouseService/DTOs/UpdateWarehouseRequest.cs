using System.ComponentModel.DataAnnotations;

namespace BestMed.WarehouseService.DTOs;

/// <summary>
/// Request DTO for updating a warehouse. All fields are optional (patch semantics).
/// </summary>
public sealed record UpdateWarehouseRequest
{
    [StringLength(50)]
    public string? Name { get; init; }

    [StringLength(100)]
    public string? Address1 { get; init; }

    [StringLength(50)]
    public string? Address2 { get; init; }

    [StringLength(50)]
    public string? Suburb { get; init; }

    [StringLength(50)]
    public string? State { get; init; }

    [StringLength(6)]
    public string? PostCode { get; init; }

    [StringLength(30)]
    public string? Country { get; init; }

    [StringLength(30)]
    public string? ContactName { get; init; }

    [StringLength(20)]
    public string? Phone { get; init; }

    [StringLength(20)]
    public string? Fax { get; init; }

    [StringLength(150)]
    public string? Email { get; init; }

    [StringLength(500)]
    public string? IPDescription { get; init; }

    [StringLength(11)]
    public string? ABN { get; init; }

    public Guid? StateTimeZoneId { get; init; }
    public bool? IsMultiSite { get; init; }
    public bool? RestrictPreferredBrand { get; init; }
    public bool? HasThirdPartyPacking { get; init; }
    public bool? PharmacyToInsert { get; init; }
    public bool? EnablePasswordAging { get; init; }
    public int? PasswordAging { get; init; }
}
