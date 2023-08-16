using Chat.Client.Helpers;
using Chat.Client.Services;
using Chat.Client.ViewModels;
using Chat.Contracts;
using Chat.Contracts.Users;

namespace Chat.Client.Views;

public partial class LoginWindow : Window
{
    /// <summary>
    /// 当登录成功时执行的动作
    /// </summary>
    public Action? SuccessAction { get; set; }
    
    public LoginWindow()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        DragControlHelper.StartDrag(Background);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        DragControlHelper.StopDrag(Background);
    }

    private void Close_OnTapped(object? sender, TappedEventArgs e)
    {
        Environment.Exit(0);
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var vm = (LoginWindowViewModel)DataContext!;
        if (string.IsNullOrEmpty(vm.Account))
        {
            return;
        }

        if (string.IsNullOrEmpty(vm.Password))
        {
            return;
        }

        var result = await MainAppHelper.GetRequiredService<IAuthService>().CreateAsync(vm.Account, vm.Password);
        if (result.Code != Constant.Success) return;
        
        Caller.SetToken(result.Data);
        MainAppHelper.GetRequiredService<StorageService>().SetToken(result.Data);
        SuccessAction?.Invoke();
        Close();
    }
}