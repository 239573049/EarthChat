using Chat.Contracts.Users;
using Chat.Service.Application.Users.Queries;
using FreeRedis;

namespace Chat.Service.Services;

public class ChatService : BaseService<ChatService>
{
    public async Task<IReadOnlyList<GetUserDto>> GetOnlineUsersAsync()
    {
        var redis = GetService<RedisClient>();
        var query = new GetUserAllQuery();
        await PublishAsync(query);
        var users = await redis!.GetAsync<IReadOnlyList<GetUserDto>>("onlineUsers");
        foreach (var userDto in query.Result)
        {
            userDto.OnLine = users?.Any(x => x.Id == userDto.Id) ?? false;
        }

        return query.Result;
    }
}