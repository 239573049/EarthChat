using Chat.Contracts.Core;
using Chat.Contracts.Users;
using Masa.Utils.Models;

namespace Chat.Contracts.Chats;

public interface IChatService
{
    Task<ResultDto<GetUserDto[]>?> GetOnlineUsersAsync();

    Task<ResultDto<PaginatedListBase<ChatMessageDto>>> GetListAsync(Guid groupId, int page, int pageSize);
}