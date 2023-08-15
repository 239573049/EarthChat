namespace Chat.Client.ViewModels;

public class LoginWindowViewModel : ViewModelBase
{
    private string _account = string.Empty;
    
    public string Account
    {
        get => _account;
        set => this.RaiseAndSetIfChanged(ref _account, value);
    }
    
    private string _password = string.Empty;
    
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
}
