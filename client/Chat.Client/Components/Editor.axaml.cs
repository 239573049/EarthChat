using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
}