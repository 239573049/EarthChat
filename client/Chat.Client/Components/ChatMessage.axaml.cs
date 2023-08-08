using System.Diagnostics;
using Chat.Client.Helpers;
using Chat.Client.ViewModels;

namespace Chat.Client.Components;

public partial class ChatMessage : UserControl
{
    private bool _startList;

    public ChatMessage()
    {
        InitializeComponent();
    }

    private ChatMessageViewModel ViewModel => (ChatMessageViewModel)DataContext;

    private void Message_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _startList = false;
    }

    private void Message_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_startList)
        {
            // 66 是标题栏高度
            ViewModel.OperatingArea = (int)(MessageMainGrid.Bounds.Height - (int)e.GetPosition(this).Y + 66);
            Debug.WriteLine((int)e.GetPosition(this).Y);
        }
    }

    private void Message_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _startList = true;
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        PathIconHelper.Path_OnPointerExited(sender, e);
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        PathIconHelper.Path_OnPointerEntered(sender, e);
    }
}