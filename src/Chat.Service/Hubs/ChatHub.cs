using System.Text.Json;
using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Commands;
using Chat.Service.Application.Chats.Queries;
using FreeRedis;
using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.Contrib.Authentication.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Hubs;

public class ChatHub : Hub
{
    private readonly IEventBus _eventBus;
    private readonly RedisClient _redisClient;

    public ChatHub(RedisClient redisClient, IEventBus eventBus)
    {
        _redisClient = redisClient;
        _eventBus = eventBus;
    }

    public override async Task OnConnectedAsync()
    {
        // 增加在线人数
        await _redisClient.IncrByAsync("online", 1);
        var userId = GetUserId();
        
        await _redisClient.LRemAsync("onlineUsers", 1, userId.ToString());

        if (userId != null)
        {
            await _redisClient.LPushAsync("onlineUsers", userId);
            var groupsQuery = new GetUserGroupQuery(userId.Value);
            await _eventBus.PublishAsync(groupsQuery);
            foreach (var groupDto in groupsQuery.Result)
            {
                // 加入群组
                await Groups.AddToGroupAsync(Context.ConnectionId, groupDto.Id.ToString("N"));
            }
        }
        else
        {
            // 默认加入群组
            await Groups.AddToGroupAsync(Context.ConnectionId, Guid.Empty.ToString("N"));
        }
        
        // 更新在线人数
        
        await UpdateOnlineAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // 减少在线人数
        await _redisClient.IncrByAsync("online", -1);

        // 移除在线用户
        var userId = GetUserId().ToString();
        if (userId != null) await _redisClient.LRemAsync("onlineUsers", 0, GetUserId().ToString());
        
        // 更新在线人数
        await UpdateOnlineAsync();

    }

    /// <summary>
    /// 通知在线人数变更
    /// </summary>
    private async Task UpdateOnlineAsync()
    {
        // 获取onlineUsers数量
        var count = await _redisClient.LLenAsync("onlineUsers");
        await Clients.All.SendAsync("UpdateOnline", count);
    }

    public async Task SendMessage(string value, int type)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return;
        }
        
        var userId = GetUserId();
        // 未登录用户不允许发送消息
        if (userId == null) return;

        string key = $"user:{userId}:count";

        // 限制用户发送消息频率
        if (await _redisClient.ExistsAsync(key))
        {
            var count = await _redisClient.GetAsync<int>(key);
            
            // 限制用户发送消息频率每分钟20条
            if (count > 20) return;
        }

        var message = new ChatMessageDto
        {
            Content = value,
            Type = (ChatType)type,
            UserId = userId.Value,
            CreationTime = DateTime.Now,
            Id = Guid.NewGuid(),
            User = new GetUserDto
            {
                Avatar = GetAvatar(),
                Id = userId.Value,
                Name = GetName()
            }
        };

        var createChat = new CreateChatMessageCommand(new CreateChatMessageDto
        {
            Content = value,
            Id = message.Id,
            Type = (ChatType)type,
            UserId = userId.Value
        });

        // 转发到客户端
        _ = Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));

        if (await _redisClient.ExistsAsync(key))
        {
            await _redisClient.IncrByAsync(key, 1);
        }
        else
        {
            await _redisClient.IncrByAsync(key, 1);
            await _redisClient.ExpireAsync(key, 60);
        }
        
        // 发送消息新增事件
        await _eventBus.PublishAsync(createChat);
        await _eventBus.CommitAsync();

        // 发送智能助手订阅事件
        var chatGPT = new ChatGPTCommand(value, base.Context.ConnectionId);
        await _eventBus.PublishAsync(chatGPT);
        await _eventBus.CommitAsync();
    }

    /// <summary>
    /// 获取当前用户id
    /// </summary>
    /// <returns></returns>
    public Guid? GetUserId()
    {
        var userId = Context.User.FindFirst(x => x.Type == ClaimType.DEFAULT_USER_ID);

        if (userId == null) return null;

        if (string.IsNullOrEmpty(userId.Value)) return null;

        return Guid.Parse(userId.Value);
    }

    /// <summary>
    /// 获取当前用户头像
    /// </summary>
    /// <returns></returns>
    private string GetAvatar()
    {
        var avatar = Context.User.FindFirst(x => x.Type == "avatar");

        return avatar?.Value == null ? "" : avatar.Value;
    }

    /// <summary>
    /// 获取当前用户名称
    /// </summary>
    /// <returns></returns>
    private string GetName()
    {
        var name = Context.User.FindFirst(x => x.Type == ClaimType.DEFAULT_USER_NAME);

        return name?.Value == null ? "" : name.Value;
    }
}