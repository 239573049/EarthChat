namespace EarthChat.Domain.Internal;

public abstract class AuditEntity<TKey, TUserId> : Entity<TKey>, ICreatable<TUserId>, IUpdatable<TUserId>
{
    public DateTime CreateTime { get; set; }
    
    public TUserId CreatableId { get; set; }
    
    public DateTime UpdateTime { get; set; }
    
    public TUserId UpdatableId { get; set; }
}