namespace Chat.Client.ViewModels;

public class ChatMessageViewModel : ViewModelBase
{
    private int operatingArea = 245;

    public int OperatingArea
    {
        get => operatingArea;
        set => this.RaiseAndSetIfChanged(ref operatingArea, value);
    }
    
    private EditorViewModel _editorViewModel = new();
    
    public EditorViewModel EditorViewModel
    {
        get => _editorViewModel;
        set => this.RaiseAndSetIfChanged(ref _editorViewModel, value);
    }
}