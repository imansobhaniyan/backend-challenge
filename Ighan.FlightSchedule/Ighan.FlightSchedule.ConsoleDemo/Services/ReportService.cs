using Ighan.FlightSchedule.ConsoleDemo.Infrastructure;
using Ighan.FlightSchedule.DataAccess;

using Microsoft.EntityFrameworkCore;

namespace Ighan.FlightSchedule.ConsoleDemo.Services;

public sealed class ReportService
{
    private readonly FlightScheduleDbContext _dbContext;

    public ReportService(FlightScheduleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateReportAsync(DateTime startDate, DateTime endDate, int agencyId)
    {
        var logger = TimeLogger.Start("Report");

        var crudeData = await GetCrudeDataAsync(startDate, endDate, agencyId);

        logger.LogDurationAndReset("Read data in:");

        var dictionaryChainToSearchIn = GenerateDictionaryToSpeedupSearch(crudeData);

        logger.LogDurationAndReset("Create directory in:");

        List<ResultDataModel> results = GenerateResults(startDate, endDate, crudeData, dictionaryChainToSearchIn);

        logger.LogDurationAndReset("Generate report in:");

        string fileDir = await SaveToFileAsync(results);

        logger.LogDurationAndReset("Report saved in:");

        logger.Finish();

        return fileDir;
    }

    private static async Task<string> SaveToFileAsync(List<ResultDataModel> results)
    {
        var fileDir = "Results";

        Directory.CreateDirectory(fileDir);

        var filePath = Path.Combine(fileDir, "results.csv");

        var lines = results.Select(f => f.ToString()).ToList();

        lines.Insert(0, "flight_id,origin_city_id,destination_city_id,departure_time,arrival_time,airline_id,status");

        await File.WriteAllLinesAsync(filePath, lines);
        return fileDir;
    }

    private static List<ResultDataModel> GenerateResults(DateTime startDate, DateTime endDate, List<CrudeDataModel> crudeData, Dictionary<int, Dictionary<int, Dictionary<int, List<DateTime>>>> dictionaryChainToSearchIn)
    {
        var results = crudeData.Where(f => startDate <= f.DepartureTime && f.DepartureTime <= endDate).Select(f => new ResultDataModel
        {
            FlightId = f.FlightId,
            OriginCityId = f.OriginCityId,
            DestinationCityId = f.DestinationCityId,
            DepartureTime = f.DepartureTime,
            ArrivalTime = f.ArrivalTime,
            AirlineId = f.AirlineId,
            Status = !hasDataWithTolerance(f.AirlineId, f.OriginCityId, f.DestinationCityId, f.PrevWeekDepartureTime)
                             ? "New"
                             : !hasDataWithTolerance(f.AirlineId, f.OriginCityId, f.DestinationCityId, f.NextWeekDepartureTime)
                                 ? "Discontinued" : string.Empty
        }).ToList();

        return results;

        bool hasDataWithTolerance(int airlineId, int originCityId, int destinationCityId, DateTime prevWeekDepartureTime)
        {
            return dictionaryChainToSearchIn[airlineId][originCityId][destinationCityId].Any(x => Math.Abs((x - prevWeekDepartureTime).TotalMinutes) <= 30);
        }
    }

    // this weird return type is being used to boost search
    private static Dictionary<int, Dictionary<int, Dictionary<int, List<DateTime>>>> GenerateDictionaryToSpeedupSearch(List<CrudeDataModel> crudeData)
    {
        return crudeData.GroupBy(f => f.AirlineId).Select(f => new
        {
            f.Key,
            OriginCityIdGroups = f.GroupBy(x => x.OriginCityId).Select(x => new
            {
                x.Key,
                DestinationCityIdGroups = x.GroupBy(y => y.DestinationCityId).Select(y => new
                {
                    y.Key,
                    DepartureTimes = y.Select(z => z.DepartureTime).ToList()
                }).ToDictionary(y => y.Key, y => y.DepartureTimes)
            }).ToDictionary(x => x.Key, x => x.DestinationCityIdGroups)
        }).ToDictionary(f => f.Key, f => f.OriginCityIdGroups);
    }

    private async Task<List<CrudeDataModel>> GetCrudeDataAsync(DateTime startDate, DateTime endDate, int agencyId)
    {
        // We need to add 7 days and 30 minutes to the start and end dates to get the full range of data
        var minDate = startDate.AddDays(-7).AddMinutes(-30);
        var maxDate = endDate.AddDays(7).AddMinutes(30);

        return await _dbContext.Flights
            .Where(f => _dbContext.Subscriptions.Any(x => x.AgencyId == agencyId && f.Route!.OriginCityId == x.OriginCityId && f.Route.DestinationCityId == x.DestinationCityId))
            .Where(f => minDate <= f.DepartureTime && f.DepartureTime <= maxDate)
            .Select(f => new CrudeDataModel
            {
                FlightId = f.FlightId,
                OriginCityId = f.Route!.OriginCityId,
                DestinationCityId = f.Route.DestinationCityId,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                AirlineId = f.AirlineId
            })
            .ToListAsync();
    }

    private class CrudeDataModel
    {
        private DateTime _departureTime;

        public required int FlightId { get; set; }
        public required int OriginCityId { get; set; }
        public required int DestinationCityId { get; set; }
        public required DateTime ArrivalTime { get; set; }
        public required int AirlineId { get; set; }
        public required DateTime DepartureTime
        {
            get { return _departureTime; }
            set
            {
                _departureTime = value;
                PrevWeekDepartureTime = _departureTime.AddDays(-7);
                NextWeekDepartureTime = _departureTime.AddDays(7);
            }
        }

        // We use these two in linq queries to avoid calculating them every time
        public DateTime PrevWeekDepartureTime { get; set; }
        public DateTime NextWeekDepartureTime { get; set; }
    }

    private class ResultDataModel
    {
        public required int FlightId { get; set; }
        public required int OriginCityId { get; set; }
        public required int DestinationCityId { get; set; }
        public required DateTime DepartureTime { get; set; }
        public required DateTime ArrivalTime { get; set; }
        public required int AirlineId { get; set; }
        public required string? Status { get; set; }

        public override string ToString()
        {
            return $"{FlightId},{OriginCityId},{DestinationCityId},{DepartureTime.ToString(DateTimeParser.DateTimeFormat)},{ArrivalTime.ToString(DateTimeParser.DateTimeFormat)},{AirlineId},{Status}";
        }
    }
}
