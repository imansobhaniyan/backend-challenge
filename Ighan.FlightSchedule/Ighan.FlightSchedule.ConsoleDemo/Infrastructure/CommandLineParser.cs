namespace Ighan.FlightSchedule.ConsoleDemo.Infrastructure;

public sealed class CommandLineParser
{
    private readonly string[] _args;

    public CommandLineParser(string[] args)
    {
        _args = args;
    }

    public int? GetInt(int index)
    {
        if (_args.Length > index && int.TryParse(_args[index], out var value))
            return value;

        return null;
    }

    public DateTime? GetDateTimeAt(int index)
    {
        if (_args.Length > index && DateTimeParser.TryParse(_args[index], out var value))
            return value;

        return null;
    }
}