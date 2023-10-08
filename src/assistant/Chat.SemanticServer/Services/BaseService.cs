using Microsoft.SemanticKernel;

namespace Chat.SemanticServer.Services;

public abstract class BaseService<T> : ServiceBase where T : class
{
    protected ILogger<T> Logger => GetRequiredService<ILogger<T>>();

    protected IKernel Kernel => GetRequiredService<IKernel>();
}