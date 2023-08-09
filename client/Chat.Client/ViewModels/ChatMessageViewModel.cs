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
    
    private ObservableCollection<GroupInUserModel> _groupInUserModels = new()
    {
        new GroupInUserModel()
        {
            Name = "次数"
        }
    };
    
    public ObservableCollection<GroupInUserModel> GroupInUserModels
    {
        get => _groupInUserModels;
        set => this.RaiseAndSetIfChanged(ref _groupInUserModels, value);
    }
}