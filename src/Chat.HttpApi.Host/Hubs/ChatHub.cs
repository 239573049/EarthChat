using Chat.Chats;
using FreeRedis;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Chat.Hubs;

public class ChatHub : Hub
{
    private readonly IRedisClient _redisClient;

    public ChatHub(IRedisClient redisClient)
    {
        _redisClient = redisClient;
    }

    public override async Task OnConnectedAsync()
    {
        await Task.CompletedTask;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Task.CompletedTask;
    }

    public async Task SendMessage(ChatMessageDto dto)
    {

    }
}