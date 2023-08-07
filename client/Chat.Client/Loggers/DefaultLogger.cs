using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Chat.Client.Loggers;

public class DefaultLogger : ILogger
{
    private string _categoryName;

    public DefaultLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
#if DEBUG
        if ((int)logLevel >= (int)LogLevel.Information)
#else
        if((int)logLevel >= (int)LogLevel.Warning)
#endif
        {
            return true;
        }

        return false;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (IsEnabled(logLevel))
        {
            var message = formatter(state, exception);

            LoggerHandler.WriteLog(logLevel, eventId, message, exception, _categoryName);

#if DEBUG
            var log =
                $"[{(_categoryName.IsNullOrEmpty() ? string.Empty : _categoryName)}{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}]  [{logLevel}] [{eventId}] [{message}]";
            log = exception?.Message is not null ? log + $" [{exception.Message}]" : log;
            Debug.WriteLine(log);
#endif
        }
    }
}