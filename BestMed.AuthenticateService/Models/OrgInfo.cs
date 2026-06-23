namespace BestMed.AuthenticateService.Models;

public sealed class OrgInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool EnablePasswordAging { get; set; }
    public int? PasswordAging { get; set; }
    public string? FacilityType { get; set; }
    public string? State { get; set; }
}
