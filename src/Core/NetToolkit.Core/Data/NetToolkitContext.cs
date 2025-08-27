using Microsoft.EntityFrameworkCore;
using NetToolkit.Core.Data.Entities;

namespace NetToolkit.Core.Data;

public class NetToolkitContext : DbContext
{
    public DbSet<ScanResult> ScanResults { get; set; }
    public DbSet<StoredScript> StoredScripts { get; set; }
    public DbSet<DeviceInfo> DeviceInfos { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public NetToolkitContext(DbContextOptions<NetToolkitContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScanResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NetworkRange).IsRequired();
            entity.Property(e => e.ResultsJson).IsRequired();
            entity.HasIndex(e => e.CreatedAt);
        });

        modelBuilder.Entity<StoredScript>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.ScriptType).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<DeviceInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IPAddress).IsRequired().HasMaxLength(45);
            entity.Property(e => e.HostName).HasMaxLength(255);
            entity.HasIndex(e => e.IPAddress).IsUnique();
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(255);
            entity.HasIndex(e => e.Timestamp);
        });

        base.OnModelCreating(modelBuilder);
    }
}