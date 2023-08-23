using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Client.ViewModels;
using Chat.Contracts;

namespace Chat.Client.Components;

public partial class Message : UserControl
{
    public Message()
    {
        InitializeComponent();

        DataContextChanged += (async (sender, args) =>
        {
            if (DataContext is not MessageListViewModel viewModel) return;

            var chatService = MainAppHelper.GetService<IChatService>();

            // 优先从缓存中获取
            if (!MainAppHelper.GetItem<IReadOnlyList<ChatGroupDto>>(Constant.GetUserGroup, out var groups))
            {
                // 从服务端获取，并且缓存。
                groups = await chatService.GetUserGroupAsync();
                MainAppHelper.AddItem(nameof(ChatGroupDto), groups);
            }

            foreach (var group in groups)
            {
                viewModel.MessageLists.Add(new MessageList()
                {
                    Avatar = group.Avatar,
                    Description = group.Description,
                    Id = group.Id,
                    Name = group.Name,
                });
            }

            UserListBox.SelectedItem = viewModel.MessageLists.First();
        });
    }

    private MessageListViewModel ViewModel => (MessageListViewModel)DataContext;

    private async void UserListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox { SelectedItem: MessageList messageList }) return;

        await SelectFirst(messageList);
    }

    public async Task SelectFirst(MessageList? messageList = null)
    {
        var eventBus = MainAppHelper.GetRequiredService<IEventBus>();

        var chatMessage = MainAppHelper.GetRequiredService<ChatMessage>();
        if (messageList != null)
        {
            await chatMessage.Select(messageList);
        }
        eventBus.Publish(EventBusConstant.ContentStackPanel, chatMessage);
    }
}