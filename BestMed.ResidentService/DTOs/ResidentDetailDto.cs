namespace BestMed.ResidentService.DTOs;

/// <summary>Full resident detail returned from GET /residents/{id}.</summary>
public sealed record ResidentDetailDto
{
    public Guid Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? DisplayName { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public Guid FacilityId { get; init; }
    public Guid? SectionId { get; init; }
    public string? Status { get; init; }
    public string? FredCode { get; init; }
    public bool? IsOtherSupplyPharmacy { get; init; }
    public Guid? AlternativeSupplyPharmacyId { get; init; }
    public bool? IsRestrictedByFacilityConfig { get; init; }
    public bool? VMCRequireTransfer { get; init; }
    public string? IHINumber { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? CreatedDate { get; init; }
    public Guid? LastUpdatedBy { get; init; }
    public DateTime? LastUpdatedDate { get; init; }
}
