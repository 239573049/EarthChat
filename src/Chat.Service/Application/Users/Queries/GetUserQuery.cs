namespace Chat.Service.Application.Users.Queries;

public record GetUserQuery(Guid userId) : Query<GetUserDto>
{
    public override GetUserDto Result { get; set; }
}