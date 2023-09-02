using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Chat.Client.Services;
using Chat.Client.ViewModels;
using DynamicData;

namespace Chat.Client.Components;

public partial class ChatMessage : UserControl
{
    private bool _startList;

    private static Dictionary<Guid, List<ChatGroupInUserDto>> _groupInUserModels = new(5);

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

    protected override void OnInitialized()
    {
        var chatHubService = MainAppHelper.GetService<ChatHubService>();
        chatHubService._receiveMessage += ReceiveMessage;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        var chatHubService = MainAppHelper.GetService<ChatHubService>();
        chatHubService._receiveMessage -= ReceiveMessage;
    }

    private void ReceiveMessage(ChatMessageDto dto)
    {
        // 使用ui更新
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (ViewModel.MessageList.Id.ToString("N") == dto.GroupId)
            {
                ViewModel.ChatMessageModels.Add(dto);
            }
        });
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

    private async void SendMessage(TappedEventArgs e)
    {
        var text = Editor.editorBox.Text;
        if (!string.IsNullOrEmpty(text))
        {
            var chatHubService = MainAppHelper.GetService<ChatHubService>();

            await chatHubService.SendMessageAsync(text, ViewModel.MessageList.Id.ToString("N"), ChatType.Text);

            ViewModel.EditorViewModel.Text = string.Empty;
        }
    }

    public async Task Select(MessageList messageList)
    {
        ViewModel.MessageList = messageList;
        if (!_groupInUserModels.TryGetValue(messageList.Id, out var list))
        {
            var chatService = await MainAppHelper.GetService<IChatService>().GetGroupInUserAsync(messageList.Id);
            if (!_groupInUserModels.ContainsKey(messageList.Id))
            {
                _groupInUserModels.TryAdd(messageList.Id, chatService);
                list = chatService;
            }
        }

        ViewModel.GroupInUserModels.Clear();
        ViewModel.GroupInUserModels.AddRange(list);
    }
}