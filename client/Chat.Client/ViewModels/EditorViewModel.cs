namespace Chat.Client.ViewModels;

public class EditorViewModel: ViewModelBase
{
    private ObservableCollection<EditorModel> _editors = new();
    
    public ObservableCollection<EditorModel> Editors
    {
        get => _editors;
        set => this.RaiseAndSetIfChanged(ref _editors, value);
    }
    
    private string _text;
    
    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }
}