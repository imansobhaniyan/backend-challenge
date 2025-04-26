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
}