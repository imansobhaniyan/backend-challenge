using Microsoft.Extensions.Configuration;

namespace Ighan.FlightSchedule.ConsoleDemo.Infrastructure;

public sealed class ConfigurationProvider
{
    private static IConfiguration? instance;

    public static IConfiguration GetInstance()
    {
        if (instance is null)
        {
            instance = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        return instance;
    }
}
