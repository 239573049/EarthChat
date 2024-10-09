namespace EarthChat.Ddd;

public abstract class Entity<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; }
}

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
