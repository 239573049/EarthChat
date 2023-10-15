using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Chat.Contracts.Core;
using Chat.Contracts.Users;
using LiteDB;
using Masa.Utils.Models;

namespace Chat.Client.Services;

public class ChatService : IChatService
{

    public Task<ResultDto<PaginatedListBase<ChatMessageDto>>> GetListAsync(Guid groupId, int page, int pageSize)
    {
        return Caller.GetHttpClient()
            .GetFromJsonAsync<ResultDto<PaginatedListBase<ChatMessageDto>>>(
                $"Chats/List?groupId={groupId}&page={page}&pageSize={pageSize}");
    }

    public Task<IReadOnlyList<ChatGroupDto>> GetUserGroupAsync()
    {
        return Caller.GetHttpClient().GetFromJsonAsync<IReadOnlyList<ChatGroupDto>>("Chats/UserGroup");
    }

    public Task<ResultDto> CreateGroupAsync(CreateGroupDto dto, string connections)
    {
        throw new NotImplementedException();
    }

    public async Task<List<GroupUserDto>> GetGroupInUserAsync(Guid groupId, int page, int pageSize)
    {
        return await Caller.GetHttpClient()
            .GetFromJsonAsync<List<GroupUserDto>>("Chats/GroupInUser?groupId=" + groupId);
    }

    public Task<ResultDto<Guid[]>> GetOnLineUserIdsAsync(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto<ChatGroupDto>> GetGroupAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto> CountermandMessage(Guid id)
    {
        throw new NotImplementedException();
    }
}