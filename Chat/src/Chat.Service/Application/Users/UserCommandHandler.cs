using AutoMapper;
using Chat.Contracts.Users;
using Chat.Service.Application.Users.Commands;
using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;
using Masa.BuildingBlocks.Data.UoW;

namespace Chat.Service.Application.Users;

public class UserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public UserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [EventHandler]
    public async Task CreateUserAsync(CreateUserCommand command)
    {
        if ((await _userRepository.GetCountAsync(x => x.Account == command.CreateUserDto.Account)) > 0)
        {
            throw new Exception("用户已存在");
        }

        var user = new User(command.CreateUserDto.Account, command.CreateUserDto.Password, command.CreateUserDto.Avatar,
            command.CreateUserDto.Name)
        {
            GiteeId = command.CreateUserDto.GiteeId,
            GithubId = command.CreateUserDto.GithubId,
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        command.Result = _mapper.Map<UserDto>(user);
        
    }
}