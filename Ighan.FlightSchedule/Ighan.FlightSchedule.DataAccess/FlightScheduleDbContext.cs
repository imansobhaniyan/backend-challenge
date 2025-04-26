using Microsoft.EntityFrameworkCore;

namespace Ighan.FlightSchedule.DataAccess;

public sealed class FlightScheduleDbContext : DbContext
{
    public FlightScheduleDbContext(DbContextOptions<FlightScheduleDbContext> options) : base(options)
    {
    }

    public DbSet<StorageModels.Flight> Flights { get; set; }
    public DbSet<StorageModels.Route> Routes { get; set; }
    public DbSet<StorageModels.Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StorageModels.Flight>()
                    .HasIndex(f => f.RouteId)
                    .IncludeProperties(f => new { f.DepartureTime, f.ArrivalTime, f.AirlineId });

        modelBuilder.Entity<StorageModels.Subscription>()
                    .HasIndex(f => f.AgencyId)
                    .IncludeProperties(f => new { f.DestinationCityId, f.OriginCityId });

        modelBuilder.Entity<StorageModels.Route>()
                    .HasIndex(f => new { f.DepartureDate, f.OriginCityId, f.DestinationCityId });
    }
}