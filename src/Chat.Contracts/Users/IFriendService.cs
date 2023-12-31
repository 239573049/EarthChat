﻿using Chat.Contracts.Chats;
using Chat.Contracts.Core;
using Masa.Utils.Models;

namespace Chat.Contracts.Users;

public interface IFriendService
{
    /// <summary>
    /// 获取好友状态
    /// </summary>
    /// <param name="friendId"></param>
    /// <returns></returns>
    Task<ResultDto<bool>> FriendStateAsync(Guid friendId);

    /// <summary>
    /// 好友申请
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<ResultDto> FriendRegistrationAsync(FriendRegistrationInput input);

    /// <summary>
    /// 获取好友申请数据
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<ResultDto<PaginatedListBase<FriendRequestDto>>> GetListAsync(int page,int pageSize);

    /// <summary>
    /// 好友申请处理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    Task<ResultDto> FriendHandleAsync(Guid id, FriendState state);
}