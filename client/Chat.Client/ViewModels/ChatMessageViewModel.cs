namespace Chat.Client.ViewModels;

public class ChatMessageViewModel : ViewModelBase
{
    private int _operatingArea = 245;

    private EditorViewModel _editorViewModel = new();

    private MessageList _messageList = new();

    public MessageList MessageList
    {
        get => _messageList;
        set => this.RaiseAndSetIfChanged(ref _messageList, value);
    }

    public int OperatingArea
    {
        get => _operatingArea;
        set => this.RaiseAndSetIfChanged(ref _operatingArea, value);
    }

    public EditorViewModel EditorViewModel
    {
        get => _editorViewModel;
        set => this.RaiseAndSetIfChanged(ref _editorViewModel, value);
    }

    private ObservableCollection<ChatGroupInUserDto> _groupInUserModels = new();

    public ObservableCollection<ChatGroupInUserDto> GroupInUserModels
    {
        get => _groupInUserModels;
        set => this.RaiseAndSetIfChanged(ref _groupInUserModels, value);
    }
}