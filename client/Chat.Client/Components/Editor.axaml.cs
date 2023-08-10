using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chat.Client.ViewModels;

namespace Chat.Client.Components;

public partial class Editor : UserControl
{
    public Editor()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InputElement_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
    }

    private void InputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBlock textBlock)
        {
            // 设置透明背景色
            textBlock.Foreground = new SolidColorBrush(Color.Parse("#00000000"));
        }
    }
    
    private EditorViewModel ViewModel => (EditorViewModel) DataContext;

    private void Send_OnTapped(object? sender, TappedEventArgs e)
    {
        
    }
}