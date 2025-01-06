namespace EarthChat.Domain.Internal;

public interface ICreatable<TUserId> : ICreatable
{
    /// <summary>
    /// 创建者Id
    /// </summary>
    public TUserId CreatableId { get; set; }
}

public interface ICreatable
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
