﻿using Chat.Contracts.Users;

namespace Chat.Contracts.Chats;

public class CreateChatMessageDto
{
    public Guid Id { get; set; }

    /// <summary>
    ///     内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     类型
    /// </summary>
    public ChatType Type { get; set; }

    /// <summary>
    ///     id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    ///     扩展参数
    /// </summary>
    public Dictionary<string, string> Extends { get; set; }

    public Guid? RevertId { get; set; }
    /// <summary>
    /// 群组Id
    /// </summary>
    public Guid ChatGroupId { get; set; }

    public GetUserDto User { get; set; }
}