using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chat.Client;

public class MainApp
{
    private IServiceCollection? _services;

    private IServiceProvider? _serviceProvider;

    public static MainApp CreateMainApp()
    {
        return new MainApp()
        {
            _services = new ServiceCollection()
        };
    }

    public static MainApp CreateMainApp(IServiceCollection services)
    {
        return new MainApp()
        {
            _services = services
        };
    }

    public IServiceProvider BuilderApp()
    {
        _serviceProvider = _services!.BuildServiceProvider();
        return _serviceProvider;
    }
}

public static class MainAppHelper
{
    private static IServiceProvider _serviceProvider = null!;

    public static Action<IServiceCollection>? AddService { get; set; }

    public static MainApp ConfigureServices(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();
        configureServices(services);
        
        AddService?.Invoke(services);

        return MainApp.CreateMainApp(services);
    }

    public static IServiceProvider BuilderApp(MainApp app)
    {
        return _serviceProvider = app.BuilderApp();
    }

    public static T GetService<T>(this MainApp app)
    {
        CheckInit();

        return _serviceProvider.GetService<T>();
    }

    public static T GetRequiredService<T>(this MainApp app)
    {
        CheckInit();
        return _serviceProvider.GetRequiredService<T>();
    }

    public static object GetService(this MainApp app, Type serviceType)
    {
        CheckInit();
        return _serviceProvider.GetService(serviceType);
    }

    public static object GetRequiredService(this MainApp app, Type serviceType)
    {
        CheckInit();
        return _serviceProvider.GetRequiredService(serviceType);
    }

    public static IEnumerable<object> GetServices(this MainApp app, Type serviceType)
    {
        CheckInit();
        return _serviceProvider.GetServices(serviceType);
    }

    public static IEnumerable<T> GetServices<T>(this MainApp app)
    {
        CheckInit();
        return _serviceProvider.GetServices<T>();
    }
    
    public static ILogger Logger(string category = "Default")
    {
        CheckInit();
        return _serviceProvider.GetService<ILoggerFactory>()!.CreateLogger(category);
    }

    public static IServiceScope CreateScope(this MainApp app)
    {
        CheckInit();
        return _serviceProvider.CreateScope();
    }

    /// <summary>
    /// 校验是否初始化
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static void CheckInit()
    {
        if (_serviceProvider == null)
        {
            // 抛出异常
            throw new Exception("请先调用BuilderApp方法");
        }
    }

    public static void Dispose(this MainApp app)
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}