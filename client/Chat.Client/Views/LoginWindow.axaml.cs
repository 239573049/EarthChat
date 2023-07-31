using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Chat.Client.Helpers;

namespace Chat.Client.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        DragControlHelper.StartDrag(Background);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        DragControlHelper.StopDrag(Background);
    }

    private void Close_OnTapped(object? sender, TappedEventArgs e)
    {
        Environment.Exit(0);
    }
}