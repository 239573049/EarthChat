using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Chat.Chat;

public class ChatGroup : FullAuditedEntity<Guid>
{
    /// <summary>
    /// 头像
    /// </summary>
    public string Avater { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 介绍
    /// </summary>
    public string Introduce { get; set; }
}