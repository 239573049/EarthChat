using System.Diagnostics;
using System.IO;
using Avalonia.Platform.Storage;
using Chat.Client.Helpers;
using Chat.Client.ViewModels;

namespace Chat.Client.Components;

public partial class ChatMessage : UserControl
{
    private bool _startList;

    public static FilePickerFileType ImageAll { get; } = new("All Images")
    {
        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp" },
        AppleUniformTypeIdentifiers = new[] { "public.image" },
        MimeTypes = new[] { "image/*" }
    };

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

    private async void InputElement_OnTapped(object? sender, TappedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择图片",
            AllowMultiple = false,
            FileTypeFilter = new[] { ImageAll }
        });

        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            await using var stream = await files[0].OpenReadAsync();
            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            // 转base64
            var base64 = Convert.ToBase64String(memoryStream.ToArray());
            ViewModel.EditorViewModel.Editors.Add(new EditorModel()
            {
                EditorType = EditorType.Image,
                Content = base64
            });
        }
    }
}