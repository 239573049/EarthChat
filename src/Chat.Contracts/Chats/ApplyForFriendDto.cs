namespace Chat.Contracts.Chats;

public class ApplyForFriendDto
{
    /// <summary>
    /// 申请描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 被申请人
    /// </summary>
    public Guid BeAppliedForId { get; set; }

    /// <summary>
    /// 申请时间
    /// </summary>
    public DateTime ApplicationDate { get; set; }
}