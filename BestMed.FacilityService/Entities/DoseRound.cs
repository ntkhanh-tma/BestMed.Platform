using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.FacilityService.Entities;

/// <summary>
/// Represents the [dbo].[DoseRound] table.
/// FK → Facility.Id, Section.Id, User.Id (OfflineBy).
/// </summary>
[Table("DoseRound")]
public class DoseRound : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid FacilityId { get; set; }

    public Guid? SectionId { get; set; }

    public DateTime DoseTime { get; set; }

    public DateTime StartOfRound { get; set; }

    public DateTime EndOfRound { get; set; }

    public int? DoseOrder { get; set; }

    public bool? SpecialRound { get; set; }

    public bool? RoundComplete { get; set; }

    public bool? Notification { get; set; }

    public DateTime? RunDate { get; set; }

    public Guid? ProcessId { get; set; }

    public bool? IsOffline { get; set; }

    public DateTime? OfflineDate { get; set; }

    /// <summary>FK → User (lives in UserService).</summary>
    public Guid? OfflineBy { get; set; }

    public bool? MissedOfflineSync { get; set; }

    public int? WarningSent { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(FacilityId))]
    public Facility? Facility { get; set; }

    [ForeignKey(nameof(SectionId))]
    public Section? Section { get; set; }
}
