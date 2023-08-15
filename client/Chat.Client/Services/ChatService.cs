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
        return Caller.GetHttpClient().GetFromJsonAsync<ResultDto<PaginatedListBase<ChatMessageDto>>>($"Chats/List?groupId={groupId}&page={page}&pageSize={pageSize}");
    }
}