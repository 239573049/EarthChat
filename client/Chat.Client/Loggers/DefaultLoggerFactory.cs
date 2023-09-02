using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Chat.Client.Loggers;

public class DefaultLoggerFactory : ILoggerFactory
{

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void AddProvider(ILoggerProvider provider)
    {
        throw new NotImplementedException();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DefaultLogger(categoryName);
    }
}

