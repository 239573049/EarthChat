namespace Chat.Contracts.Chats;

public enum FriendState
{
    /// <summary>
    /// 申请中
    /// </summary>
    ApplyFor,
    
    /// <summary>
    /// 同意申请
    /// </summary>
    Consent,
    
    /// <summary>
    /// 拒绝
    /// </summary>
    Refuse
}