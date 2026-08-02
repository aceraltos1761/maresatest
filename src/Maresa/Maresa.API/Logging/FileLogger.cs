namespace Maresa.API.Logging;

public class FileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly string _filePath;
    private readonly object _writeLock;

    public FileLogger(string categoryName, string filePath, object writeLock)
    {
        _categoryName = categoryName;
        _filePath = filePath;
        _writeLock = writeLock;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_categoryName}: {formatter(state, exception)}";

        if (exception is not null)
        {
            line += Environment.NewLine + exception;
        }

        lock (_writeLock)
        {
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }
    }
}
