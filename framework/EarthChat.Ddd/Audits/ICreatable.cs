namespace EarthChat.Ddd;


public interface ICreatable
{
    DateTime CreatedAt { get; set; }
}


public interface ICreatable<TUser> : ICreatable
{
    TUser? CreatedBy { get; set; }
}