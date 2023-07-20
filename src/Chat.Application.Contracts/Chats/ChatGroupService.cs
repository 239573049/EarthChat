using Chat.Chats.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Chats;

public interface IChatGroupService
{
    /// <summary>
    /// 创建新的群聊
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task CreateAsync(CreateChatGroupDto input);

    /// <summary>
    /// 更新群聊基本信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task UpdateAsync(ChatGroupDto input);

    /// <summary>
    /// 删除群聊
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// 获取当前用户群聊列表
    /// </summary>
    /// <returns></returns>
    Task<List<ChatGroupDto>> GetListAsync();

    /// <summary>
    /// 加入群聊
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task AddGroupChatAsync(Guid groupId, Guid userId);
}