using System.ComponentModel.Design;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Chat.Client.Components;
using Chat.Client.Loggers;
using Chat.Client.Services;
using Chat.Client.ViewModels;
using Chat.Client.ViewModels.Users;
using Chat.Client.Views;
using Chat.Client.Views.Users;
using Chat.Contracts;
using Chat.Contracts.Files;
using Chat.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chat.Client;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
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
            
            service.AddSingleton(new CreateGroupWindow()
            {
                DataContext = new CreateGroupViewModel(),
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

            service.AddSingleton<ChatMessage>((_) => new ChatMessage()
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
            
            
        });

        var app = mainApp.BuilderApp();

        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        var storage = app.GetRequiredService<StorageService>();
        if (storage.GetToken().IsNullOrWhiteSpace())
        {
            DesktopLogin(desktop);
        }
        else
        {
            try
            {
                Caller.SetToken(storage.GetToken());
                // 如果token存在，尝试获取用户信息，如果获取失败，重新登录
                var userService = app.GetRequiredService<IUserService>();
                var user = await userService.GetAsync();
                if (user.Code == Constant.Success)
                {
                    desktop.MainWindow = app.GetRequiredService<MainWindow>();
                    desktop.MainWindow.Show();
                }
                else
                {
                    DesktopLogin(desktop);
                }
            }
            catch (Exception e)
            {
                MainAppHelper.Logger().LogError("获取用户信息失败 Message:{e}", e);
                DesktopLogin(desktop);
            }
        }
    }

    private static void DesktopLogin(IClassicDesktopStyleApplicationLifetime desktop)
    {
        var loginWindow = MainAppHelper.GetRequiredService<LoginWindow>();
        loginWindow.Show();

        loginWindow.SuccessAction = () =>
        {
            desktop.MainWindow = MainAppHelper.GetRequiredService<MainWindow>();
            desktop.MainWindow.Show();
        };

        desktop.MainWindow = loginWindow;
    }
}