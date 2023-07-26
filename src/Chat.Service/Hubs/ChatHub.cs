using System.Text.Json;
using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Commands;
using FreeRedis;
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

        if (userId != null) await _redisClient.LPushAsync("onlineUsers", userId);
        
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
            if (count > 5) return;
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


        await _eventBus.PublishAsync(createChat);
    }

    public Guid? GetUserId()
    {
        var userId = Context.User.FindFirst(x => x.Type == ClaimType.DEFAULT_USER_ID);

        if (userId == null) return null;

        if (string.IsNullOrEmpty(userId.Value)) return null;

        return Guid.Parse(userId.Value);
    }

    private string GetAvatar()
    {
        var avatar = Context.User.FindFirst(x => x.Type == "avatar");

        return avatar?.Value == null ? "" : avatar.Value;
    }

    private string GetName()
    {
        var name = Context.User.FindFirst(x => x.Type == ClaimType.DEFAULT_USER_NAME);

        return name?.Value == null ? "" : name.Value;
    }
}