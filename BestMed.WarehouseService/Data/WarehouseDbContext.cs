using BestMed.Data;
using BestMed.WarehouseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.WarehouseService.Data;

/// <summary>
/// Database-first DbContext for read-write operations.
/// Inherits from <see cref="BestMedDbContext"/> for automatic audit timestamps,
/// soft-delete query filters, and other shared conventions.
/// Re-scaffold with:
///   dotnet ef dbcontext scaffold "Name=ConnectionStrings:warehousedb" Microsoft.EntityFrameworkCore.SqlServer
///       --output-dir Entities --context-dir Data --context WarehouseDbContext --force --no-onconfiguring
/// </summary>
public partial class WarehouseDbContext : BestMedDbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Warehouse> Warehouses { get; set; } = null!;
    public virtual DbSet<WarehouseBankDetail> BankDetails { get; set; } = null!;
    public virtual DbSet<WarehouseDocument> Documents { get; set; } = null!;
    public virtual DbSet<WarehouseHoliday> Holidays { get; set; } = null!;
    public virtual DbSet<WarehouseRobot> Robots { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
