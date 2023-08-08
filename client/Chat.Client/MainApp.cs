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

    public IServiceProvider Builder()
    {
        _serviceProvider = _services!.BuildServiceProvider();
        return _serviceProvider;
    }
}

public static class MainAppHelper
{
    private static IServiceProvider _serviceProvider = null!;

    public static Action<IServiceCollection>? AddService { get; set; }

    private static MainApp _mainApp;

    public static MainApp ConfigureServices(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();
        configureServices(services);

        AddService?.Invoke(services);

        return _mainApp = MainApp.CreateMainApp(services);
    }

    public static IServiceProvider BuilderApp(this MainApp app)
    {
        return _serviceProvider = app.Builder();
    }

    public static T GetService<T>()
    {
        CheckInit();

        return _serviceProvider.GetService<T>();
    }

    public static T GetRequiredService<T>()
    {
        CheckInit();
        return _serviceProvider.GetRequiredService<T>();
    }

    public static object GetService(Type serviceType)
    {
        CheckInit();
        return _serviceProvider.GetService(serviceType);
    }

    public static object GetRequiredService(Type serviceType)
    {
        CheckInit();
        return _serviceProvider.GetRequiredService(serviceType);
    }

    public static IEnumerable<object> GetServices(Type serviceType)
    {
        CheckInit();
        return _serviceProvider.GetServices(serviceType);
    }

    public static IEnumerable<T> GetServices<T>()
    {
        CheckInit();
        return _serviceProvider.GetServices<T>();
    }

    public static ILogger Logger(string category = "Default")
    {
        CheckInit();
        return _serviceProvider.GetService<ILoggerFactory>()!.CreateLogger(category);
    }

    public static IServiceScope CreateScope()
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