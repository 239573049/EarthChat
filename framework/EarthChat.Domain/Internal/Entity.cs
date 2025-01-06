namespace EarthChat.Domain.Internal;

public class Entity<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; }
}
