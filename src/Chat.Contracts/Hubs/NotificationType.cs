namespace Chat.Contracts.Hubs;

public enum NotificationType
{
    /// <summary>
    /// 系统通知
    /// </summary>
    System,
    
    /// <summary>
    /// 在线人数
    /// </summary>
    Online,
    
    /// <summary>
    /// 好友申请
    /// </summary>
    FriendRequest,
    
    /// <summary>
    /// 群聊上线新用户
    /// </summary>
    GroupUserNew,
    
    /// <summary>
    /// 群聊新增用户
    /// </summary>
    GroupAppendUser,
    
    /// <summary>
    /// 撤回消息
    /// </summary>
    Countermand,
    
    /// <summary>
    /// 用户下线
    /// </summary>
    GroupInOffLine
}