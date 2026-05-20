using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[BCPSetting] table — 1:1 with Pharmacy. No audit columns.
/// </summary>
[Table("BCPSetting")]
public class BCPSetting : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PharmacyId { get; set; }

    [StringLength(100)]
    public string? DropboxToken { get; set; }

    public string? OneDriveToken { get; set; }

    public DateTime? OneDriveTokenExpired { get; set; }

    public DateTime? OneDriveExpired { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy Pharmacy { get; set; } = null!;
}
