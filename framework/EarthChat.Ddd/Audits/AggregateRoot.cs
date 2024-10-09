namespace EarthChat.Ddd;

public interface IAggregateRoot<TKey> : IEntity<TKey>, IHasTenant, ISoftDelete, IExtend
{
}

public interface IAggregateRoot<TKey, TUser> : IAggregateRoot<TKey>, IUpdatable<TUser>
{
}

public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
{
    public string? TenantId { get; set; }

    public bool IsDeleted { get; set; }

    public IDictionary<string, string> Extend { get; set; }
}