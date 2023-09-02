using System.ComponentModel.Design;
using System.IO;
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
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chat.Client;

public partial class App : Application
{
    private static readonly MainApp MainApp;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

        var storage = MainAppHelper.GetRequiredService<StorageService>();
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
                var userService = MainAppHelper.GetRequiredService<IUserService>();
                var user = await userService.GetAsync();
                if (user.Code == Constant.Success)
                {
                    desktop.MainWindow = MainAppHelper.GetRequiredService<MainWindow>();
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