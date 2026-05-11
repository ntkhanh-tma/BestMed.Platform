using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PrescriberService.Entities;

/// <summary>
/// Represents the [dbo].[Prescriber] table. Database-first — do not modify manually.
/// Re-scaffold when schema changes.
/// </summary>
[Table("Prescriber")]
public partial class Prescriber : IEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string PrescriberName { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string PrescriberCode { get; set; } = null!;

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(20)]
    public string? Fax { get; set; }

    [StringLength(20)]
    public string? OutOfHours { get; set; }

    [StringLength(100)]
    public string? Address1 { get; set; }

    [StringLength(100)]
    public string? Address2 { get; set; }

    [StringLength(50)]
    public string? Suburb { get; set; }

    [StringLength(10)]
    public string? State { get; set; }

    [StringLength(4)]
    public string? Postcode { get; set; }

    [StringLength(50)]
    public string? Country { get; set; }

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(100)]
    public string? PINHash { get; set; }

    [StringLength(100)]
    public string? PINSalt { get; set; }

    public int? EmailNotifications { get; set; }

    [StringLength(20)]
    public string? MobileNumber { get; set; }

    [StringLength(20)]
    public string? HPIINumber { get; set; }

    [StringLength(50)]
    public string? HPIIStatus { get; set; }

    [StringLength(13)]
    public string? AHPRANumber { get; set; }

    public bool PinAcknowledge { get; set; }

    [Column(TypeName = "varchar(20)")]
    [StringLength(20)]
    public string? LicenseNumber { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ClusteredKey { get; set; }

    [StringLength(30)]
    public string? Qualification { get; set; }

    [StringLength(100)]
    public string? DefaultMimsSeverityLevel { get; set; }

    [StringLength(100)]
    public string? DefaultMimsDocumentationLevel { get; set; }

    public bool EnableMimsDrugInteractionChecking { get; set; }

    public bool? IseRxUserAccessAgreementAccepted { get; set; }

    public DateTime? ERxUserAccessAgreementAcceptedDate { get; set; }

    [StringLength(200)]
    public string? ERxUserAccessAgreementVersion { get; set; }

    [StringLength(200)]
    public string? ERxEntityId { get; set; }

    [StringLength(50)]
    public string? PreferredName { get; set; }
}
