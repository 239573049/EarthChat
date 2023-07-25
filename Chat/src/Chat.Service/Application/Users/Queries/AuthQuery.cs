namespace Chat.Service.Application.Users.Queries;

public record AuthQuery(string Account, string Password) : Query<UserDto>
{
    public override UserDto Result { get; set; }
}