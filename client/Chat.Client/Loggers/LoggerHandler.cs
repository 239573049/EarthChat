using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Chat.Client.Loggers;

public class LoggerHandler
{
    // 定义消费管道
    private static readonly Channel<LoggerModule> _channel = Channel.CreateUnbounded<LoggerModule>();

    static LoggerHandler()
    {
        // 开启消费管道
        Start();
    }

    public static bool WriteLog(LogLevel logLevel, EventId eventId, string message, Exception? exception, string categoryName)
    {
        // 将日志写入管道
        return _channel.Writer.TryWrite(new LoggerModule()
        {
            LogLevel = logLevel,
            CategoryName = categoryName,
            Message = message,
            EventId = eventId,
            Exception = exception
        });
    }

    private static void Start()
    {
        // 开启消费管道
        Task.Run(async () =>
            {
                while (await _channel.Reader.WaitToReadAsync())
                {
                    while (_channel.Reader.TryRead(out var module))
                    {
                        try
                        {
                            WriteFile(module);
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine(e);
                        }
                    }
                }
            }
        );
    }

    private static void WriteFile(LoggerModule module)
    {
        var fileName = Path.Combine(AppContext.BaseDirectory, "logs", $"{DateTime.Now:yyyy-MM-dd}.log");
        var log =
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{module.CategoryName} {module.LogLevel}] [{module.EventId}] [{module.Exception?.Message}]";
        
        var info = new FileInfo(fileName);
        
        if (module.Exception is not null)
        {
            log += $" [{module.Exception.Message}]";
            log += $" [{module.Exception.StackTrace}]";
        }

        if(info.Directory is not null && !info.Directory.Exists) info.Directory.Create();
        
        File.AppendAllLines(fileName, new[] { log });
    }
}

public class LoggerModule
{
    public LogLevel LogLevel
    {
        get => logLevel;
        set => logLevel = value;
    }

    public EventId EventId
    {
        get => eventId;
        set => eventId = value;
    }

    public Exception? Exception
    {
        get => exception;
        set => exception = value;
    }

    public string Message
    {
        get => message;
        set => message = value;
    }

    public string CategoryName
    {
        get => categoryName;
        set => categoryName = value;
    }
    
    private string categoryName;
    private LogLevel logLevel;
    private EventId eventId;
    private Exception? exception;
    private string message;
}