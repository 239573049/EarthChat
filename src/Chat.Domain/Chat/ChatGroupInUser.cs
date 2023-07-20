using Chat.Users;
using System;
using Volo.Abp.Domain.Entities;

namespace Chat.Chat;

public class ChatGroupInUser : Entity<Guid>
{
    public Guid ChatGroupId { get; set; }

    public Guid UserId { get; set; }

    public ChatGroup ChatGroup { get; set; }

    public ChatUser User { get; set; }
}