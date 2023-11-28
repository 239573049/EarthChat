using Chat.Contracts.Chats;
using Chat.Contracts.Hubs;
using Chat.Service.Application.Chats.Commands;
using Chat.Service.Application.Chats.Queries;
using Chat.Service.Application.Hubs.Commands;
using Chat.Service.Infrastructure.Helper;
using Chat.Service.Services;
using Masa.Contrib.Authentication.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Hubs;

public class ChatHub(RedisClient redisClient, IEventBus eventBus, BackgroundTaskService backgroundTaskService,
        ILogger<ChatHub> logger)
    : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        // 在首次链接的时候将当前用户和链接id进行关联
        await redisClient.SetAsync(Constant.OnLineKey + userId.Value.ToString("N"), userId.Value);
        await redisClient.LPushAsync("Connections:" + userId.Value, Context.ConnectionId);

        // 通过事件获取到用户所有的群组
        var groupsQuery = new GetUserGroupQuery(userId.Value, null);
        await eventBus.PublishAsync(groupsQuery);

        // 在这里将当前的SignalR的链接id加入到获取的groupId中，这样就只有这个group的成员才能相互发送消息。
        foreach (var groupDto in groupsQuery.Result)
        {
            var key = Constant.Group.GroupUsers + groupDto.Id.ToString("N");
            // 加入群组
            await Groups.AddToGroupAsync(Context.ConnectionId, groupDto.Id.ToString("N"));

            // 如果用户不存在当前群聊在线人数中，则添加。
            await redisClient.LRemAsync(key, -1, userId);
            await redisClient.LPushAsync(key, userId);
        }

        var systemCommand = new SystemCommand(new Notification()
        {
            createdTime = DateTime.Now,
            type = NotificationType.GroupUserNew,
            content = "新人用户上线",
            data = userId
        }, groupsQuery.Result.Select(x => x.Id).ToArray(), true);

        // 在这里将通知前端有新的用户上线。
        await eventBus.PublishAsync(systemCommand);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // 移除在线用户
        var userId = GetUserId();
        if (userId.HasValue)
        {
            // 当集合的数量少于对于1则当前链接是最后一个
            if ((await redisClient.LRangeAsync<string>(Constant.Connections + userId, 0, 2)).Length <= 1)
            {
                // 需要去掉在线标识
                await redisClient.DelAsync(Constant.OnLineKey + userId.Value.ToString("N"));

                // 获取当前用户所在的链接群
                var groupsQuery = new GetUserGroupQuery(userId.Value, null);
                await eventBus.PublishAsync(groupsQuery);

                // 退出所有链接
                foreach (var groupDto in groupsQuery.Result)
                {
                    var key = Constant.Group.GroupUsers + groupDto.Id.ToString("N");
                    // 加入群组
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupDto.Id.ToString("N"));

                    // 清空当前用户群的在线列表
                    await redisClient.LRemAsync(key, -1, userId);
                }

                var systemCommand = new SystemCommand(new Notification()
                {
                    createdTime = DateTime.Now,
                    type = NotificationType.GroupInOffLine,
                    content = "用户下线",
                    data = userId
                }, groupsQuery.Result.Select(x => x.Id).ToArray(), true);

                await eventBus.PublishAsync(systemCommand);
            }

            // 移除当前链接
            await redisClient.LRemAsync(Constant.Connections + userId, -1, Context.ConnectionId);
        }
    }

    /// <summary>
    /// 群聊消息转发
    /// </summary>
    /// <param name="value"></param>
    /// <param name="groupId"></param>
    /// <param name="type"></param>
    /// <param name="revertId"></param>
    public async Task SendMessage(string value, Guid groupId, int type, Guid? revertId = null, bool group = true)
    {
        try
        {
            if (value.IsNullOrWhiteSpace())
            {
                return;
            }

            var userId = GetUserId();
            // 未登录用户不允许发送消息
            if (userId == null || userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException();
            }

            value = SensitiveWordsAc.Instance.TakeOutSensitive(value);

            string key = $"user:{userId}:count";

            // 限制用户发送消息频率
            if (await redisClient.ExistsAsync(key))
            {
                var count = await redisClient.GetAsync<int>(key);

                // 限制用户发送消息频率每分钟20条
                if (count > 20) return;
            }

            var message = new ChatMessageDto
            {
                Content = value,
                Type = (ChatType)type,
                UserId = userId.Value,
                CreationTime = DateTime.Now,
                RevertId = revertId,
                GroupId = groupId,
                Id = Guid.NewGuid()
            };

            var createChat = new CreateChatMessageCommand(new CreateChatMessageDto
            {
                Content = value,
                Id = message.Id,
                RevertId = revertId,
                ChatGroupId = groupId,
                Type = (ChatType)type,
                UserId = userId.Value
            }, group);

            // 如果发送的内容关联了回复id则查询回复内容
            if (message.RevertId != null && message.RevertId != Guid.Empty)
            {
                var messageQuery = new GetMessageQuery((Guid)message.RevertId);
                await eventBus.PublishAsync(messageQuery);
                message.Revert = messageQuery.Result;
            }

            // 为当前用户增加发送数量，以便限制用户的发送频率
            if (await redisClient.ExistsAsync(key))
            {
                await redisClient.IncrByAsync(key, 1);
            }
            else
            {
                await redisClient.IncrByAsync(key, 1);
                await redisClient.ExpireAsync(key, 60);
            }

            // 发送消息新增事件
            await eventBus.PublishAsync(createChat);

            // 转发到客户端
            _ = Clients.Groups(groupId.ToString("N")).SendAsync("ReceiveMessage", groupId, message);

            // 发送智能助手订阅事件
            await backgroundTaskService.WriteAsync(new AssistantDto()
            {
                Id = groupId,
                Value = value,
                Group = true,
                RevertId = message.Id,
                UserId = userId.Value
            });
        }
        catch (Exception e)
        {
            logger.LogError("发送消息出现异常：{e}", e);
        }
    }

    /// <summary>
    /// 获取当前用户id
    /// </summary>
    /// <returns></returns>
    public Guid? GetUserId()
    {
        var userId = Context.User?.FindFirst(x => x.Type == ClaimType.DEFAULT_USER_ID);

        if (userId == null) return null;

        if (string.IsNullOrEmpty(userId.Value)) return null;

        return Guid.Parse(userId.Value);
    }
}