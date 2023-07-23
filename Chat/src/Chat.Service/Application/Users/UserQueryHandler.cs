using AutoMapper;
using Chat.Contracts.Users;
using Chat.Service.Application.Users.Queries;
using Chat.Service.Domain.Users.Repositories;

namespace Chat.Service.Application.Users;

public class UserQueryHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [EventHandler]
    public async Task AuthAsync(AuthQuery query)
    {
        var user = await _userRepository.FindAsync(x => x.Account == query.Account);

        if (user is null)
        {
            throw new Exception("用户不存在");
        }

        if (user.Password != query.Password)
        {
            throw new Exception("密码错误");
        }

        query.Result = _mapper.Map<UserDto>(user);
    }
}