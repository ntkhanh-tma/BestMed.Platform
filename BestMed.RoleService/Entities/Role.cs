using BestMed.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestMed.RoleService.Entities;

/// <summary>
/// Represents the [dbo].[UserRole] table. Database-first — do not modify manually.
/// Re-scaffold when schema changes.
/// </summary>
[Table("UserRole")]
public partial class Role : IEntity
{
    [Key]
    public Guid Id { get; set; }

    [Column("Role")]
    [StringLength(50)]
    public string? RoleCode { get; set; }

    [StringLength(150)]
    public string? RoleName { get; set; }

    [StringLength(250)]
    public string? Description { get; set; }

    public Guid? UserTypeId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ClusteredKey { get; set; }

    public string? ConcurrencyStamp { get; set; }

    [StringLength(50)]
    public string? NormalizedRole { get; set; }
}
