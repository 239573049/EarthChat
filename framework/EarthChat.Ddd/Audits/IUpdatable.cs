namespace EarthChat.Ddd;

public interface IUpdatable
{
    DateTime? UpdatedAt { get; set; }
}

public interface IUpdatable<TUser> : IUpdatable
{
    TUser? UpdatedBy { get; set; }
}