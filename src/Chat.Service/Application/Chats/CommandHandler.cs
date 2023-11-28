using System.Diagnostics;
using Chat.Contracts.Chats;
using Chat.Contracts.Hubs;
using Chat.Service.Application.Chats.Commands;
using Chat.Service.Application.Hubs.Commands;
using Chat.Service.Application.System.Commands;
using Chat.Service.Application.Third_party.Queries;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Application.Chats;

public class CommandHandler(IChatMessageRepository chatMessageRepository, IUserContext userContext,
    IHttpClientFactory httpClientFactory, IHubContext<ChatHub> hubContext, IEventBus eventBus,
    ILogger<CommandHandler> logger, IChatGroupRepository chatGroupRepository, IUnitOfWork unitOfWork,
    IChatGroupInUserRepository chatGroupInUserRepository, IFriendRepository friendRepository,
    IFriendRequestRepository friendRequestRepository)
{
    [EventHandler]
    public async Task CreateAsync(CreateChatMessageCommand command)
    {
        var chatMessage = new ChatMessage(command.Dto.Id, DateTime.Now)
        {
            Content = command.Dto.Content,
            Type = command.Dto.Type,
            RevertId = command.Dto.RevertId,
            ChatGroupId = command.Dto.ChatGroupId,
            UserId = command.Dto.UserId
        };

        await chatMessageRepository.AddAsync(chatMessage);
        await unitOfWork.SaveChangesAsync();
    }

    [EventHandler(1)]
    public async Task UpdateNewMessage(CreateChatMessageCommand command)
    {
        if (command.Group)
        {

            await chatGroupRepository.UpdateNewMessage(command.Dto.ChatGroupId, command.Dto.Content);
        }
        else
        {
            await friendRepository.UpdateNewMessage(command.Dto.ChatGroupId, command.Dto.Content);
        }
    }

    [EventHandler]
    public async Task CreateGroupAsync(CreateGroupCommand command)
    {
        if (await chatGroupRepository.GetCountAsync(x => x.Creator == userContext.GetUserId<Guid>()) > 10)
            throw new UserFriendlyException("最多只能创建10个群组");

        var chatGroup = new ChatGroup(Guid.NewGuid())
        {
            Avatar = command.Dto.Avatar,

            Description = command.Dto.Description ?? string.Empty,
            Name = command.Dto.Name,
        };

        await chatGroupRepository.AddAsync(chatGroup);

        var chatGroupInUser = new ChatGroupInUser()
        {
            ChatGroupId = chatGroup.Id,
            UserId = userContext.GetUserId<Guid>()
        };

        await chatGroupInUserRepository.AddAsync(chatGroupInUser);

        // 新建群聊的时候想要将当前链接加入群聊。
        await hubContext.Groups.AddToGroupAsync(command.connections, chatGroup.Id.ToString("N"));
    }

    [EventHandler]
    public async Task InvitationGroupAsync(InvitationGroupCommand command)
    {
        if (await chatGroupInUserRepository.GetCountAsync(x =>
                x.ChatGroupId == command.id && x.UserId == userContext.GetUserId<Guid>()) > 0)
        {
            throw new UserFriendlyException("您已经加入群聊");
        }

        if (await chatGroupRepository.GetCountAsync(x => x.Id == command.id) <= 0)
        {
            throw new UserFriendlyException("群聊不存在");
        }

        await chatGroupInUserRepository.AddAsync(new ChatGroupInUser()
        {
            ChatGroupId = command.id,
            UserId = userContext.GetUserId<Guid>()
        });
    }

    [EventHandler]
    public async Task ApplyForFriendAsync(ApplyForFriendCommand command)
    {
        if (await friendRepository.GetCountAsync(x => x.FriendId == command.Dto.BeAppliedForId) > 0)
        {
            throw new UserFriendlyException("已经存在好友关系");
        }

        if (await friendRequestRepository.GetCountAsync(x =>
                x.RequestId == userContext.GetUserId<Guid>() && x.State == FriendState.ApplyFor) > 0)
        {
            throw new UserFriendlyException("已经存在申请");
        }

        var value = new FriendRequest()
        {
            ApplicationDate = DateTime.Now,
            RequestId = userContext.GetUserId<Guid>(),
            BeAppliedForId = command.Dto.BeAppliedForId,
            Description = command.Dto.Description,
            State = FriendState.ApplyFor
        };

        await friendRequestRepository.AddAsync(value);

        var systemCommand = new SystemCommand(new Notification()
        {
            createdTime = DateTime.Now,
            type = NotificationType.FriendRequest,
            content = "发起新的好友申请",
        }, new[] { command.Dto.BeAppliedForId }, false);
        await eventBus.PublishAsync(systemCommand);
    }

    [EventHandler]
    public async Task ApplicationProcessingAsync(ApplicationProcessingCommand command)
    {
        var value = await friendRequestRepository.FindAsync(x => x.Id == command.Id);

        if (value?.State == FriendState.ApplyFor)
        {
            if (command.State == FriendState.Consent)
            {
                await friendRepository.AddAsync(new Friend()
                {
                    SelfId = userContext.GetUserId<Guid>(),
                    FriendId = value.RequestId
                });

                await friendRepository.AddAsync(new Friend()
                {
                    SelfId = value.RequestId,
                    FriendId = userContext.GetUserId<Guid>()
                });

                var systemCommand = new SystemCommand(new Notification()
                {
                    createdTime = DateTime.Now,
                    type = NotificationType.FriendRequest,
                    content = "同意了好友申请",
                }, new[] { value.RequestId }, false);
                await eventBus.PublishAsync(systemCommand);
            }

            value.State = command.State;

            await friendRequestRepository.UpdateAsync(value);
        }
    }

    [EventHandler]
    public async Task CountermandAsync(CountermandCommand command)
    {
        var value = await chatMessageRepository.FindAsync(x =>
            x.Id == command.Id && x.Creator == userContext.GetUserId<Guid>());

        if (value == null)
        {
            return;
        }

        // 判断消息创建时间是否超过5分钟
        if (value.CreationTime < DateTime.Now.AddMinutes(-5))
        {
            throw new UserFriendlyException("消息超过5分钟不能撤回");
        }

        if (await chatMessageRepository.UpdateCountermand(value.Id, userContext.GetUserId<Guid>(), true))
        {
            var systemCommand = new SystemCommand(new Notification()
            {
                createdTime = DateTime.Now,
                content = "撤回消息",
                data = value.Id,
                type = NotificationType.Countermand
            }, new[] { value.ChatGroupId },
                true);

            await eventBus.PublishAsync(systemCommand);

            if (value.Type is ChatType.File or ChatType.Audio or ChatType.Image or ChatType.Video)
            {
                var deleteFile = new DeleteFileSystemCommand(value.Content);
                await eventBus.PublishAsync(deleteFile);
            }
        }
        else
        {
            throw new UserFriendlyException("撤回失败");
        }
    }
}