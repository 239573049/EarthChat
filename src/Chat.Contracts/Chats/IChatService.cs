using Chat.Contracts.Core;
using Chat.Contracts.Users;
using Masa.Utils.Models;

namespace Chat.Contracts.Chats;

public interface IChatService
{
    /// <summary>
    /// 获取当前在线用户。
    /// </summary>
    /// <returns></returns>
    Task<ResultDto<GetUserDto[]>?> GetOnlineUsersAsync();

    /// <summary>
    /// 获取指定群组的聊天记录。
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<ResultDto<PaginatedListBase<ChatMessageDto>>> GetListAsync(Guid groupId, int page, int pageSize);

    /// <summary>
    /// 获取当前用户的所有群组。
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<ChatGroupDto>> GetUserGroupAsync();

    /// <summary>
    /// 创建群组。
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<ResultDto> CreateGroupAsync(CreateGroupDto dto);
    
    /// <summary>
    /// 将用户添加到群组。
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task AddUserToGroupAsync(Guid groupId, Guid userId);

    /// <summary>
    /// 获取群组中的用户。
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    Task<IOrderedEnumerable<UserDto>> GetGroupInUserAsync(Guid groupId);

    /// <summary>
    /// 通过id获取指定群聊基本信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ResultDto<ChatGroupDto>> GetGroupAsync(Guid id);
}