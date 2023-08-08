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

            service.AddSingleton<MainWindow>((_) => new MainWindow()
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
                        DataContext = new UserManageViewModel()
                    };
                }
                throw new CheckoutException("未找到MainWindow");
            });
            
            #region Views

            service.AddSingleton<ChatMessage>((_)=>new ChatMessage()
            {
                DataContext = new ChatMessageViewModel()
            });
            
            service.AddSingleton<Message>((_) => new Message()
            {
                DataContext = new MessageListViewModel()
            });
            service.AddSingleton<UserManage>((_) => new UserManage()
            {
                DataContext = new UserManageViewModel()
            });
            
            #endregion
            
            
            service.AddSingleton<IEventBus, EventBus>();
        });

        var app = mainApp.BuilderApp();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = app.GetRequiredService<MainWindow>();
        }
    }
}