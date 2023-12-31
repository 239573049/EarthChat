﻿using Chat.Contracts.Chats;
using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;

namespace Chat.Service.Domain.Users.Aggregates;

public class FriendRequest : FullAggregateRoot<Guid,Guid>
{
    /// <summary>
    /// 申请描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 申请状态
    /// </summary>
    public FriendState State { get; set; }

    /// <summary>
    /// 申请人
    /// </summary>
    public Guid RequestId { get; set; }

    /// <summary>
    /// 被申请人
    /// </summary>
    public Guid BeAppliedForId { get; set; }

    /// <summary>
    /// 申请时间
    /// </summary>
    public DateTime ApplicationDate { get; set; }

}