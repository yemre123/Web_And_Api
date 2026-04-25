using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<FanSchedule> FanSchedules => Set<FanSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var fanSchedule = modelBuilder.Entity<FanSchedule>();

        fanSchedule.ToTable("FanSchedules");
        fanSchedule.HasKey(x => x.DeviceName);
        fanSchedule.Property(x => x.DeviceName).HasColumnName("devicename").HasMaxLength(128);
        fanSchedule.Property(x => x.StopStartTime)
            .HasColumnName("stop_start_time")
            .HasConversion(
                value => value.ToString("HH:mm:ss"),
                value => TimeOnly.ParseExact(value, "HH:mm:ss"));
        fanSchedule.Property(x => x.StopEndTime)
            .HasColumnName("stop_end_time")
            .HasConversion(
                value => value.ToString("HH:mm:ss"),
                value => TimeOnly.ParseExact(value, "HH:mm:ss"));
        fanSchedule.Property(x => x.RunSeconds).HasColumnName("run_seconds");
        fanSchedule.Property(x => x.StopSeconds).HasColumnName("stop_seconds");
        //fanSchedule.Property(x => x.ColForceStop).HasColumnName("col_force_stop");
        fanSchedule.Property(x => x.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(true);

        fanSchedule.HasCheckConstraint("CK_FanSchedules_RunSeconds_Positive", "run_seconds > 0");
        fanSchedule.HasCheckConstraint("CK_FanSchedules_StopSeconds_NonNegative", "stop_seconds >= 0");
    }
}
