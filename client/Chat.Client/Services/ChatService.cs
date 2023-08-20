using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Chat.Contracts.Core;
using Chat.Contracts.Users;
using Masa.Utils.Models;

namespace Chat.Client.Services;

public class ChatService : IChatService
{
    public Task<ResultDto<GetUserDto[]>?> GetOnlineUsersAsync()
    {
        return Caller.GetHttpClient().GetFromJsonAsync<ResultDto<GetUserDto[]>>("Chats/OnlineUsers");
    }

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

    /// <inheritdoc />
    public async Task CreateGroupAsync(CreateGroupDto dto)
    {
        await Caller.GetHttpClient().PostAsJsonAsync("Chats/Group", dto);
    }

    public Task AddUserToGroupAsync(Guid groupId, Guid userId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<List<ChatGroupInUserDto>> GetGroupInUserAsync(Guid groupId)
    {
        return await Caller.GetAsync<List<ChatGroupInUserDto>>("Chats/GroupInUser?groupId=" + groupId);
    }
}