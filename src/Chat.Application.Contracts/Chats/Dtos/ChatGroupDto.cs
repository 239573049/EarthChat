using System;
using Volo.Abp.Application.Dtos;

namespace Chat.Chats.Dtos;

public class ChatGroupDto : FullAuditedEntityDto<Guid>
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