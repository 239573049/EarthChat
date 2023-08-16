namespace Chat.Client.ViewModels.Users;

public class CreateGroupViewModel : ViewModelBase
{
    private CreateGroupDto _createGroupDto = new();

    public CreateGroupDto CreateGroupDto
    {
        get => _createGroupDto;
        set => this.RaiseAndSetIfChanged(ref _createGroupDto, value);
    }
}