using Chat.Chat;
using System;
using Volo.Abp.Application.Dtos;

namespace Chat.Chats;

public class ChatMessageDto : FullAuditedEntityDto<Guid>
{

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 群组Id
    /// </summary>
    public Guid ChatGroupId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public ChatType Type { get; set; }
}