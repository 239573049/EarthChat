using Chat.Contracts.Users;

namespace Chat.Service.Application.Users.Commands;

public record CreateUserCommand(CreateUserDto CreateUserDto) : Command
{
    public UserDto Result { get; set; }
}