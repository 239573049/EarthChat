namespace EarthChat.Domain.Internal;

public interface IEntity<TKey>
{
    public TKey Id { get; set; }
}