namespace Chat.Client.ViewModels;

public class ChatMessageViewModel : ViewModelBase
{
    private int operatingArea = 245;

    public int OperatingArea
    {
        get => operatingArea;
        set => this.RaiseAndSetIfChanged(ref operatingArea, value);
    }
}