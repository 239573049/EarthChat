using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Commands;
using Chat.Service.Application.Chats.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Service.Services;

public class ChatService : BaseService<ChatService>, IChatService
{
    [Authorize]
    public async Task<ResultDto<PaginatedListBase<ChatMessageDto>>> GetListAsync(Guid groupId, int page, int pageSize)
    {
        var query = new GeChatMessageListQuery(groupId, page, pageSize);
        await PublishAsync(query);
        return query.Result.CreateResult();
    }

    [Authorize]
    public async Task<IReadOnlyList<ChatGroupDto>> GetUserGroupAsync(bool? group)
    {
        var userContext = GetRequiredService<IUserContext>();
        var query = new GetUserGroupQuery(userContext.GetUserId<Guid>(),group);
        await PublishAsync(query);
        return query.Result;
    }

    [Authorize]
    public async Task<ResultDto> CreateGroupAsync(CreateGroupDto dto, string connections)
    {
        var command = new CreateGroupCommand(dto, connections);

        await PublishAsync(command);

        return new ResultDto();
    }

    public async Task<List<GroupUserDto>> GetGroupInUserAsync(Guid groupId, int page, int pageSize)
    {
        var userIds =
            await RedisClient.LRangeAsync<Guid>(Constant.Group.GroupUsers + groupId.ToString("N"), page - 1, pageSize);

        var query = new GetGroupInUserQuery(groupId, page, pageSize, userIds);
        
        await PublishAsync(query);
        
        return query.Result.Select(x => new GroupUserDto(x.Id, userIds.Contains(x.Id))).ToList();
    }

    [Authorize]
    public async Task<ResultDto<Guid[]>> GetOnLineUserIdsAsync(Guid groupId)
    {
        var userIds = await RedisClient.LRangeAsync<Guid>(Constant.Group.GroupUsers + groupId.ToString("N"), 0, -1) ??
                      Array.Empty<Guid>();

        return userIds.CreateResult();
    }

    [Authorize]
    public async Task<ResultDto<ChatGroupDto>> GetGroupAsync(Guid id)
    {
        var query = new GetGroupQuery(id);
        await PublishAsync(query);
        return query.Result.CreateResult();
    }

    /// <inheritdoc />
    public async Task<ResultDto> CountermandMessage(Guid id)
    {
        var command = new CountermandCommand(id);
        await PublishAsync(command);

        return new ResultDto();
    }

    [Authorize]
    public async Task InvitationGroupAsync(Guid id)
    {
        var command = new InvitationGroupCommand(id);
        await PublishAsync(command);
    }
}