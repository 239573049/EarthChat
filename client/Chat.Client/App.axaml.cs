using System.ComponentModel.Design;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Chat.Client.Components;
using Chat.Client.Loggers;
using Chat.Client.ViewModels;
using Chat.Client.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chat.Client;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var mainApp = MainAppHelper.ConfigureServices(service =>
        {
            service.AddHttpClient();
            service.AddSingleton<ILoggerFactory, DefaultLoggerFactory>();

            service.AddSingleton(new MainWindow()
            {
                DataContext = new MainWindowViewModel(),
            });

            service.AddSingleton(new LoginWindow()
            {
                DataContext = new LoginWindowViewModel(),
            });

            service.AddSingleton((serviceProvider) =>
            {
                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                if (mainWindow.DataContext is MainWindowViewModel view)
                {
                    return new UserManage()
                    {
                        DataContext = view.MessageListViewModel
                    };
                }
                throw new CheckoutException("未找到MainWindow");
            });
            
            service.AddSingleton((serviceProvider) =>
            {
                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                if (mainWindow.DataContext is MainWindowViewModel view)
                {
                    return new Message()
                    {
                        DataContext = view.MessageListViewModel
                    };
                }
                throw new CheckoutException("未找到MainWindow");
            });
        });

        var app = mainApp.BuilderApp();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = app.GetRequiredService<MainWindow>();
        }
    }
}