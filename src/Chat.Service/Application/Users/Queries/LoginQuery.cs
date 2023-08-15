namespace Chat.Service.Application.Users.Queries;

public record GetUserByAccountQuery(string account, string password) : Query<UserDto>
{
    public override UserDto Result { get; set; }
}