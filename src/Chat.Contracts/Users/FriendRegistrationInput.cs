namespace Chat.Contracts.Users;

public class FriendRegistrationInput
{
    /// <summary>
    /// 申请描述
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// 被申请人
    /// </summary>
    public Guid BeAppliedForId { get; set; }
}