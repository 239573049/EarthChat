using Chat.Contracts.Users;

namespace Chat.Service.Application.Users.Queries;

public record AuthTypeQuery(string type, string id) : Query<UserDto?>
{
    public override UserDto? Result { get; set; }
}