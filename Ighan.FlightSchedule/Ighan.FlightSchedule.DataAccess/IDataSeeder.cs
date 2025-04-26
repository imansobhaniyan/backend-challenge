namespace Ighan.FlightSchedule.DataAccess;

public interface IDataSeeder
{
    bool IsDataSeedNeeded(FlightScheduleDbContext dbContext);

    void SeedRoutes(Func<FlightScheduleDbContext> dbContextGenerator);
    void SeedFlights(Func<FlightScheduleDbContext> dbContextGenerator);
    void SeedSubscriptions(Func<FlightScheduleDbContext> dbContextGenerator);
}