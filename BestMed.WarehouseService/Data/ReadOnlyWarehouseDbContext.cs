using BestMed.Data;
using BestMed.WarehouseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestMed.WarehouseService.Data;

/// <summary>
/// Read-only DbContext for the WarehouseService.
/// Uses a separate connection string pointing to a read replica.
/// Change tracking is disabled and SaveChanges is blocked.
/// </summary>
public partial class ReadOnlyWarehouseDbContext : BestMedReadOnlyDbContext
{
    public ReadOnlyWarehouseDbContext(DbContextOptions<ReadOnlyWarehouseDbContext> options) : base(options)
    {
    }

    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<WarehouseBankDetail> BankDetails { get; set; } = null!;
    public DbSet<WarehouseDocument> Documents { get; set; } = null!;
    public DbSet<WarehouseHoliday> Holidays { get; set; } = null!;
    public DbSet<WarehouseRobot> Robots { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
