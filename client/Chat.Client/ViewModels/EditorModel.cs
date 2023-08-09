namespace Chat.Client.ViewModels;

public class EditorModel : ViewModelBase
{
    private EditorType _editorType;

    public EditorType EditorType
    {
        get => _editorType;
        set => this.RaiseAndSetIfChanged(ref _editorType, value);
    }

    private string _content;

    public string Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }
}