using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Local projection of [dbo].[BESTMEDSupplyPharmacy] — Facility–Pharmacy supply config.
/// The canonical definition lives in PharmacyService.
/// </summary>
[Table("BESTMEDSupplyPharmacy")]
public class BESTMEDSupplyPharmacy : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PharmacyId { get; set; }

    public Guid FacilityId { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }
}
