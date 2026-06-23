namespace BestMed.AuthenticateService.Models;

public sealed class UserIdentification
{
    public List<OrgInfo> Facilities { get; set; } = [];
    public List<OrgInfo> Pharmacies { get; set; } = [];
    public List<OrgInfo> WareHouses { get; set; } = [];
    public List<Guid> SupplyPharmacyFacilities { get; set; } = [];
    public List<Guid> SupplyPharmacyFacilityPharmacies { get; set; } = [];
    public List<string> FeatureScreens { get; set; } = [];
    public string Mode { get; set; } = string.Empty;
}
