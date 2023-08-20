using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Commands;
using Chat.Service.Application.Chats.Queries;

namespace Chat.Service.Services;

public class ChatService : BaseService<ChatService>, IChatService
{
    public async Task<ResultDto<GetUserDto[]>?> GetOnlineUsersAsync()
    {
        var redis = GetService<RedisClient>();
        var query = new GetUserAllQuery();
        await PublishAsync(query);
        var users = await redis?.LRangeAsync<Guid>("onlineUsers", 0, -1);
        foreach (var userDto in query.Result) userDto.OnLine = users?.Any(x => x == userDto.Id) ?? false;

        return (query.Result.OrderByDescending(x => x.OnLine).ToArray()).CreateResult();
    }

    public async Task<ResultDto<PaginatedListBase<ChatMessageDto>>> GetListAsync(Guid groupId, int page, int pageSize)
    {
        var query = new GeChatMessageListQuery(groupId, page, pageSize);
        await PublishAsync(query);
        return query.Result.CreateResult();
    }

    public async Task<IReadOnlyList<ChatGroupDto>> GetUserGroupAsync()
    {
        var userContext = GetRequiredService<IUserContext>();
        var query = new GetUserGroupQuery(userContext.GetUserId<Guid>());
        await PublishAsync(query);
        return query.Result;
    }

    public async Task CreateGroupAsync(CreateGroupDto dto)
    {
        var command = new CreateGroupCommand(dto);

        await PublishAsync(command);
    }

    public Task AddUserToGroupAsync(Guid groupId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ChatGroupInUserDto>> GetGroupInUserAsync(Guid groupId)
    {
        var query = new GetGroupInUserQuery(groupId);
        await PublishAsync(query);
        return query.Result;
    }
}