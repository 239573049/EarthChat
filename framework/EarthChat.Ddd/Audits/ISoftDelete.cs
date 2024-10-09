namespace EarthChat.Ddd;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}