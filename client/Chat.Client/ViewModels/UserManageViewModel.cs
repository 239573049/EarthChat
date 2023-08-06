namespace Chat.Client.ViewModels;

public class UserManageViewModel : ViewModelBase
{
    private ObservableCollection<Users> _users = new();
    
    public ObservableCollection<Users> Users
    {
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }
}