using Chat.Contracts.Core;
using Chat.Contracts.Users;
using Masa.Utils.Models;

namespace Chat.Contracts.Chats;

public interface IChatService
{
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
    /// <param name="connections"></param>
    /// <returns></returns>
    Task<ResultDto> CreateGroupAsync(CreateGroupDto dto,string connections);

    /// <summary>
    /// 获取群组中的用户。
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    Task<List<GroupUserDto>> GetGroupInUserAsync(Guid groupId);

    /// <summary>
    /// 只获取群组在线用户id
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    Task<ResultDto<IEnumerable<Guid>>> GetOnLineUserIdsAsync(Guid groupId); 

    /// <summary>
    /// 通过id获取指定群聊基本信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ResultDto<ChatGroupDto>> GetGroupAsync(Guid id);

    /// <summary>
    /// 撤回消息
    /// </summary>
    /// <param name="id">消息Id</param>
    /// <returns></returns>
    Task<ResultDto> CountermandMessage(Guid id);
}