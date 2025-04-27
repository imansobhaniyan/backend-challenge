using Ighan.FlightSchedule.DataAccess;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace Ighan.FlightSchedule.ConsoleDemo.Infrastructure;

public sealed class DataSeeder : IDataSeeder
{
    public void SeedSubscriptions(Func<FlightScheduleDbContext> dbContextGenerator)
    {
        using (var dbContext = dbContextGenerator.Invoke())
        {
            var values = ReadLines("subscriptions.csv").Select(line => $"({line})");

            var query = "INSERT INTO [Subscriptions] ([AgencyId], [OriginCityId], [DestinationCityId]) VALUES " + string.Join(",", values);

            dbContext.Database.ExecuteSqlRaw(query);
        }

        GC.Collect();
    }

    public void SeedRoutes(Func<FlightScheduleDbContext> dbContextGenerator)
    {
        // did not use 'using' because we need to generate dbContext later
        var dbContext = dbContextGenerator.Invoke();
        {
            var values = GetInsertValues("routes").Select(line => $"({line})").ToList();

            for (int i = 0; i <= values.Count / 1_000; i++)
            {
                var query = "SET IDENTITY_INSERT Routes ON; INSERT INTO [Routes] ([RouteId], [OriginCityId], [DestinationCityId], [DepartureDate]) VALUES " + string.Join(",", values.Skip(i * 1_000).Take(1_000));

                dbContext.Database.ExecuteSqlRaw(query);

                // regenerate dbContext every 10_000 records to boost insert
                if (i != 0 && i % 10 == 0)
                {
                    dbContext.Dispose();
                    dbContext = dbContextGenerator.Invoke();
                }
            }

            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Routes OFF;");
        }

        dbContext.Dispose();

        GC.Collect();
    }

    public void SeedFlights(Func<FlightScheduleDbContext> dbContextGenerator)
    {
        {
            var values = GetInsertValues("flights").Select(line => $"({line})").ToList();

            int bigPage = 500_000, smallPage = 1_000;

            Parallel.For(0, (values.Count / bigPage) + 1, bigPageIndex =>
            {
                var pigPageValues = values.Skip(bigPageIndex * bigPage).Take(bigPage).ToList();

                // did not use 'using' because we need to generate dbContext later
                var dbContext = dbContextGenerator.Invoke();

                for (var i = 0; i <= pigPageValues.Count / smallPage; i++)
                {
                    var thisPageValues = pigPageValues.Skip(i * smallPage).Take(smallPage).ToList();

                    if (thisPageValues.Count == 0)
                        continue;

                    var query = "SET IDENTITY_INSERT Flights ON; INSERT INTO [Flights] ([FlightId], [RouteId], [DepartureTime], [ArrivalTime], [AirlineId]) VALUES " + string.Join(",", thisPageValues);

                    dbContext.Database.ExecuteSqlRaw(query);

                    // regenerate dbContext every 10_000 records to boost insert
                    if (i != 0 && i % 10 == 0)
                    {
                        dbContext.Dispose();
                        dbContext = dbContextGenerator.Invoke();
                    }
                }

                dbContext.Dispose();
            });

            using var dbContext = dbContextGenerator.Invoke();

            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Flights OFF;");
        }
        
        GC.Collect();
    }

    private IEnumerable<string> GetInsertValues(string fileName)
    {
        var lines = ReadLines($"{fileName}.csv");

        lines = lines.Select(line =>
        {
            if (line.Contains("-") is false)
                return line;

            var values = line.Split(",");

            for (int i = 0; i < values.Length; i++)
            {
                // add '' for date values
                if (values[i].Contains("-"))
                    values[i] = $"'{values[i]}'";
            }

            return string.Join(",", values);
        });

        return lines;
    }

    private IEnumerable<string> ReadLines(string fileName)
    {
        return File.ReadLines(Path.Combine("Data", fileName)).Skip(1);
    }

    public bool IsDataSeedNeeded(FlightScheduleDbContext dbContext)
    {
        return dbContext.Flights.Any() is false;
    }
}