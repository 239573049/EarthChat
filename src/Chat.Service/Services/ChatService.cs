using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Commands;
using Chat.Service.Application.Chats.Queries;

namespace Chat.Service.Services;

public class ChatService : BaseService<ChatService>, IChatService
{
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

    public async Task<ResultDto> CreateGroupAsync(CreateGroupDto dto, string connections)
    {
        var command = new CreateGroupCommand(dto, connections);

        await PublishAsync(command);

        return new ResultDto();
    }

    public Task AddUserToGroupAsync(Guid groupId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IOrderedEnumerable<UserDto>> GetGroupInUserAsync(Guid groupId)
    {
        var query = new GetGroupInUserQuery(groupId);
        await PublishAsync(query);
        return query.Result.OrderByDescending(x => x.OnLine);
    }

    public async Task<ResultDto<IEnumerable<Guid>>> GetOnLineUserIdsAsync(Guid groupId)
    {
        var query = new GetGroupInUserQuery(groupId);
        await PublishAsync(query);
        var redis = GetService<RedisClient>();
        var values =
            await redis.MGetAsync<Guid?>(query.Result.Select(x => Constant.OnLineKey + x.Id.ToString("N")).ToArray());
        values = values.Where(x => x.HasValue).ToArray();
        return query.Result.Where(x => values.Any(y => x.Id == y)).Select(x => x.Id).CreateResult();
    }

    public async Task<ResultDto<ChatGroupDto>> GetGroupAsync(Guid id)
    {
        var query = new GetGroupQuery(id);
        await PublishAsync(query);
        return query.Result.CreateResult();
    }

    public async Task InvitationGroupAsync(Guid id)
    {
        var command = new InvitationGroupCommand(id);
        await PublishAsync(command);
    }
}