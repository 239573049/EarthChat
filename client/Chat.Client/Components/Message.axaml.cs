using System.Collections.Generic;
using System.Linq;
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

    private void UserListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox { SelectedItem: MessageList messageList }) return;

        SelectFirst();
    }

    public void SelectFirst()
    {
        var eventBus = MainAppHelper.GetRequiredService<IEventBus>();

        eventBus.Publish(EventBusConstant.ContentStackPanel, MainAppHelper.GetRequiredService<ChatMessage>());
    }
}