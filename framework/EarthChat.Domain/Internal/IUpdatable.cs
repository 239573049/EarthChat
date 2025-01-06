namespace EarthChat.Domain.Internal;

public interface IUpdatable<TUserId> : IUpdatable
{
    /// <summary>
    /// 更新者Id
    /// </summary>
    public TUserId UpdatableId { get; set; }
}


public interface IUpdatable
{
    public DateTime UpdateTime { get; set; }
}
