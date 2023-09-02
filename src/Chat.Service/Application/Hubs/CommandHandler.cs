using Chat.Service.Application.Hubs.Commands;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Application.Hubs;

public class CommandHandler
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly RedisClient _redisClient;

    public CommandHandler(IHubContext<ChatHub> hubContext, RedisClient redisClient)
    {
        _hubContext = hubContext;
        _redisClient = redisClient;
    }

    [EventHandler]
    public async Task SystemAsync(SystemCommand command)
    {
        if (command.group)
        {
            await _hubContext.Clients.Groups(command.ids.Select(x => x.ToString("N")))
                .SendAsync("Notification", command.Notification);
        }
        else
        {
            var connections = new List<string>();
            foreach (var notificationId in command.ids)
            {
                connections.AddRange(await _redisClient.LRangeAsync("Connections:" + notificationId, 0, -1));
            }

            await _hubContext.Clients.Clients(connections).SendAsync("Notification", command.Notification);
        }
    }
}