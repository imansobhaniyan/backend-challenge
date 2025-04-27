namespace Ighan.FlightSchedule.ConsoleDemo.Infrastructure;

public static class DateTimeParser
{
    public const string DateFormat = "yyyy-MM-dd";

    public static bool TryParse(string? stringValue, out DateTime dateTime)
    {
        return DateTime.TryParseExact(stringValue, DateFormat, null, System.Globalization.DateTimeStyles.None, out dateTime);
    }
}
