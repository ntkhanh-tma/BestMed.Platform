using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[BESTtrackFacilityConfig] table — S8/controlled drug tracking config per Facility.
/// Has LastUpdatedBy/LastUpdateDateUTC only (no CreatedDate/CreatedBy).
/// </summary>
[Table("BESTtrackFacilityConfig")]
public class BESTtrackFacilityConfig : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    public int TimeOut { get; set; }

    public bool IsDualSign { get; set; }

    public int CheckOutTime { get; set; } = 8;

    public int PromptsNumber { get; set; } = 1;

    public int StockTakePeriod { get; set; }

    [StringLength(20)]
    public string? StockTakeFrequency { get; set; }

    public int StockTakeS8Period { get; set; }

    [StringLength(20)]
    public string? StockTakeS8Frequency { get; set; }

    [StringLength(1000)]
    public string? NotificationMethod { get; set; }

    public int? NotificationGraceTime { get; set; }

    public bool DiscrepancyNotifications { get; set; }

    [StringLength(1000)]
    public string? UsersNotify { get; set; }

    public bool? EnablePrescriberInstructions { get; set; }

    public bool EnableTransferFromBook { get; set; }

    public bool TrackS4DMedications { get; set; } = true;

    public bool? CommentChkOutMandatory { get; set; }

    public bool DestroyMedication { get; set; } = true;

    public bool EnableBalanceBottles { get; set; }

    public bool EnablePrescriberNameMandatory { get; set; }

    public bool? ReEnterPINForDualSigning { get; set; }

    public bool? PasswordAndPINTransaction { get; set; }

    public bool? EnableBESTTrackDoseFormUnit { get; set; }

    public bool? DisableDualSignIndividualStockCount { get; set; }

    public bool? BCPChartNotifications { get; set; }

    public Guid LastUpdatedBy { get; set; }

    public DateTime LastUpdateDateUTC { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }
}
