using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.PharmacyService.Entities;

/// <summary>
/// Represents the [dbo].[PharmacyInvoiceDocument] table.
/// FK→ Pharmacy.Id, Document.Id (external).
/// </summary>
[Table("PharmacyInvoiceDocument")]
public class PharmacyInvoiceDocument : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PharmacyId { get; set; }

    public Guid DocumentId { get; set; }

    [Required]
    [StringLength(20)]
    public string InvoiceNumber { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string InvoiceStatus { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public long ClusteredKey { get; set; }

    // Navigation
    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy Pharmacy { get; set; } = null!;
}
