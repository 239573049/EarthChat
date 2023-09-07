namespace Chat.Service.Domain.Users.Aggregates;

public class Emoji : Entity<Guid>, IAuditEntity<Guid>
{
    /// <summary>
    /// 表情包地址
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid Modifier { get; set; }

    public DateTime ModificationTime { get; set; }
}