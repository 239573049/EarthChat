namespace Chat.Contracts.Chats;

public class GroupUserDto
{
    public GroupUserDto(Guid userId, bool onLine)
    {
        UserId = userId;
        OnLine = onLine;
    }

    public Guid UserId { get; set; }

    /// <summary>
    /// 是否在线
    /// </summary>
    public bool OnLine { get; set; }
}