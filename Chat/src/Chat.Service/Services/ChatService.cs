using Chat.Contracts.Chats;
using Chat.Contracts.Core;
using Chat.Contracts.Users;
using Chat.Service.Application.Chats.Queries;
using Chat.Service.Application.Users.Queries;
using FreeRedis;

namespace Chat.Service.Services;

public class ChatService : BaseService<ChatService>
{
    public async Task<ResultDto<IReadOnlyList<GetUserDto>>> GetOnlineUsersAsync()
    {
        var redis = GetService<RedisClient>();
        var query = new GetUserAllQuery();
        await PublishAsync(query);
        var users = await redis?.LRangeAsync<Guid>("onlineUsers",0, -1);
        foreach (var userDto in query.Result)
        {
            userDto.OnLine = users?.Any(x => x == userDto.Id) ?? false;
        }

        return new ResultDto<IReadOnlyList<GetUserDto>>(query.Result.OrderByDescending(x=>x.OnLine).ToArray());
    }

    public async Task<ResultDto<PaginatedListBase<ChatMessageDto>>> GetListAsync(int page, int pageSize)
    {
        var query = new GeChatMessageListQuery(page, pageSize);
        await PublishAsync(query);
        return new ResultDto<PaginatedListBase<ChatMessageDto>>(query.Result);
    }
}