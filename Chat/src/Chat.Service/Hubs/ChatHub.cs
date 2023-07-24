using FreeRedis;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Hubs;

public class ChatHub : Hub
{
    private readonly RedisClient _redisClient;

    public ChatHub(RedisClient redisClient)
    {
        _redisClient = redisClient;
    }

    public override async Task OnConnectedAsync()
    {
        // 增加在线人数
        await _redisClient.IncrByAsync("online", 1);

        await _redisClient.LPushAsync("onlineUsers", GetUserId().ToString());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // 减少在线人数
        await _redisClient.IncrByAsync("online", -1);
        
        // 移除在线用户
        await _redisClient.LRemAsync("onlineUsers", 0, GetUserId().ToString());
    }

    public Guid GetUserId()
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("用户未登录");
        }

        return Guid.Parse(userId);
    }
}