namespace Ighan.FlightSchedule.ConsoleDemo.Infrastructure;

public sealed class ConsoleReader
{
    public static int? ReadInt(string message)
    {
        int? result = null;

        do
        {
            Console.Write(message);
            var value = Console.ReadLine();

            if (value == "exit")
                break;

            if (int.TryParse(value, out var dateTime))
                result = dateTime;
        } while (result is null);

        return result;
    }

    public static DateTime? ReadDateTime(string message)
    {
        DateTime? result = null;

        do
        {
            Console.Write(message);
            var value = Console.ReadLine();

            if (value == "exit")
                break;

            if (DateTimeParser.TryParse(value, out var dateTime))
                result = dateTime;
        } while (result is null);

        return result;
    }
}
