namespace Chat.Client.ViewModels;

public class UserManageViewModel : ViewModelBase
{
    private ObservableCollection<Models.Users> _users = new();
    
    public ObservableCollection<Models.Users> Users
    {
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }
}