using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Chat.Chat;

public class ChatMessage : FullAuditedEntity<Guid>
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