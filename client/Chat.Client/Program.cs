using System.ComponentModel.Design;
using System.IO;
using System.Security.Cryptography;
using Avalonia;
using Avalonia.ReactiveUI;
using Chat.Client.Components;
using Chat.Client.Loggers;
using Chat.Client.Services;
using Chat.Client.ViewModels;
using Chat.Client.ViewModels.Users;
using Chat.Client.Views;
using Chat.Client.Views.Users;
using Chat.Contracts.Files;
using Chat.Contracts.Users;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chat.Client;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        
        var mainApp = MainAppHelper.ConfigureServices(service =>
        {
            service.AddHttpClient();
            service.AddSingleton<StorageService>();
            service.AddSingleton<ILoggerFactory, DefaultLoggerFactory>();

            service.AddSingleton<MainWindow>((_) => new MainWindow()
            {
                DataContext = new MainWindowViewModel(),
            });

            service.AddSingleton((_)=>new CreateGroupWindow()
            {
                DataContext = new CreateGroupViewModel(),
            });

            service.AddSingleton((_)=>new LoginWindow()
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

            service.AddSingleton<ChatMessage>((_) =>
                new ChatMessage()
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

            service.AddSingleton<IChatService, ChatService>();
            service.AddSingleton<IAuthService, AuthService>();
            service.AddSingleton<IUserService, UserService>();
            service.AddSingleton<IFileService, FileService>();

            service.AddSingleton<ChatHubService>();

            service.AddSingleton((_) =>
            {
                var connectionString =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Chat",
                        "file.db");
                return new LiteDatabase(connectionString);
            });

            service.AddSingleton((_) => MD5.Create());
        });
        
        mainApp.BuilderApp();
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}