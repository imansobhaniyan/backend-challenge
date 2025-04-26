using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Ighan.FlightSchedule.DataAccess;

public sealed class FlightScheduleDbContextProvider
{
    private static FlightScheduleDbContext? instance;

    public static FlightScheduleDbContext GetInstance(IConfiguration configuration, IDataSeeder dataSeeder)
    {
        if (instance is null)
        {
            instance = getDbInstance();

            instance.Database.Migrate();

            if (dataSeeder.IsDataSeedNeeded(instance))
            {
                dataSeeder.SeedSubscriptions(getDbInstance);
                dataSeeder.SeedRoutes(getDbInstance);
                dataSeeder.SeedFlights(getDbInstance);
            }
        }

        return instance;

        FlightScheduleDbContext getDbInstance()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FlightScheduleDbContext>()
                .UseSqlServer(configuration.GetRequiredSection("ConnectionStrings")["Default"]);

            return new FlightScheduleDbContext(optionsBuilder.Options);
        }
    }
}