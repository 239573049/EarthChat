namespace Chat.Client.ViewModels;

public class MessageListViewModel : ViewModelBase
{
    private ObservableCollection<MessageList> _messageLists = new();
    
    public ObservableCollection<MessageList> MessageLists
    {
        get => _messageLists;
        set => this.RaiseAndSetIfChanged(ref _messageLists, value);
    }
}