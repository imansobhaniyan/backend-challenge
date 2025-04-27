using Ighan.FlightSchedule.ConsoleDemo.Infrastructure;
using Ighan.FlightSchedule.ConsoleDemo.Services;
using Ighan.FlightSchedule.DataAccess;

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

using var db = FlightScheduleDbContextProvider.GetInstance(ConfigurationProvider.GetInstance(), new DataSeeder());

var service = new ReportService(db);

await service.GenerateReportAsync(startDate.Value, endDate.Value, agencyId.Value);

Console.Write("Press any key to exit"); Console.ReadLine();