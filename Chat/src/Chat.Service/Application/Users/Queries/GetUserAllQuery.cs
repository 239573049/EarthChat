using Chat.Contracts.Users;

namespace Chat.Service.Application.Users.Queries;

public record GetUserAllQuery() : Query<IReadOnlyList<GetUserDto>>
{
    public override IReadOnlyList<GetUserDto> Result { get; set; }
}