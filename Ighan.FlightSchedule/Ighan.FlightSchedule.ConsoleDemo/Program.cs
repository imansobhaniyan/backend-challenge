using Ighan.FlightSchedule.ConsoleDemo.Infrastructure;
using Ighan.FlightSchedule.ConsoleDemo.Services;
using Ighan.FlightSchedule.DataAccess;

using System.Diagnostics;

var commands = new CommandLineParser(args);

var startDate = commands.GetDateTimeAt(0) ?? ConsoleReader.ReadDateTime($"Please enter start date ({DateTimeParser.DateFormat}) or 'exit': ");

if (startDate is null)
    return;

var endDate = (commands.GetDateTimeAt(1) ?? ConsoleReader.ReadDateTime($"Please enter end date ({DateTimeParser.DateFormat}) or 'exit': "))?.AddDays(1);

if (endDate is null)
    return;

var agencyId = commands.GetInt(2) ?? ConsoleReader.ReadInt("Please enter agency id or 'exit': ");

if (agencyId is null)
    return;

var serviceLogger = TimeLogger.Start("Program");

using var db = FlightScheduleDbContextProvider.GetInstance(ConfigurationProvider.GetInstance(), new DataSeeder());

serviceLogger.LogDurationAndReset("db context inited in:");

var service = new ReportService(db);

await service.GenerateReportAsync(startDate.Value, endDate.Value, agencyId.Value);

serviceLogger.LogDuration("Report generated in:");
serviceLogger.Finish();

Console.WriteLine();
Console.WriteLine("Choose what to do next: ");
Console.WriteLine("1. Open file with notepad");
Console.WriteLine("2. Open file directory");
Console.WriteLine("3. Exit");

var input = ConsoleReader.ReadInt("Enter the option: ");

if (input is null)
    return;

switch (input)
{
    case 1:
        Process.Start(new ProcessStartInfo
        {
            FileName = "notepad.exe",
            Arguments = Path.Combine("Results", "results.csv"),
            UseShellExecute = true
        });
        break;
    case 2:
        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = Path.Combine(Directory.GetCurrentDirectory(), "Results"),
            UseShellExecute = true
        });
        break;
    default:
        return;
}