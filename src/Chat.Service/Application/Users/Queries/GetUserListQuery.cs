namespace Chat.Service.Application.Users.Queries;

public record GetUserListQuery(List<Guid> UserIds) : Query<List<UserDto>>
{
    public override List<UserDto> Result { get; set; }
}