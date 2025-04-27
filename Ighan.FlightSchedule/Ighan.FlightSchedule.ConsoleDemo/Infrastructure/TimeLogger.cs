namespace Ighan.FlightSchedule.ConsoleDemo.Infrastructure;

public class TimeLogger
{
    private readonly DateTime _createTime;
    private DateTime _startTime;
    private readonly string? _messagePrefix;

    private TimeLogger(string? messagePrefix)
    {
        _createTime = _startTime = DateTime.Now;
        _messagePrefix = messagePrefix;

        Log("Started");
    }

    public void LogDuration(string message)
    {
        Log($"{message} {DateTime.Now - _startTime:ss\\.fff}");
    }

    public void LogDurationAndReset(string message)
    {
        LogDuration(message);

        _startTime = DateTime.Now;
    }

    public void Finish()
    {
        _startTime = _createTime;
        LogDuration("Finished in:");
    }

    private void Log(string message)
    {
        Console.WriteLine($"{_messagePrefix} - {message}".Trim(' ', '-'));
    }

    public static TimeLogger Start(string? messagePrefix)
    {
        return new TimeLogger(messagePrefix);
    }
}