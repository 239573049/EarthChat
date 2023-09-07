using AutoMapper;
using Chat.Service.Domain.Users.Repositories;

namespace Chat.Service.Application.Users;

public class UserQueryHandler
{
    private readonly IMapper _mapper;
    private readonly RedisClient _redisClient;
    private readonly IUserContext _userContext;
    private readonly IEmojiRepository _emojiRepository;
    private readonly IUserRepository _userRepository;

    public UserQueryHandler(IUserRepository userRepository, IMapper mapper, RedisClient redisClient,
        IEmojiRepository emojiRepository, IUserContext userContext)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        _emojiRepository = emojiRepository;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task AuthAsync(AuthQuery query)
    {
        var user = await _userRepository.FindAsync(x => x.Account == query.Account);

        if (user is null) throw new Exception("用户不存在");

        if (user.Password != query.Password) throw new Exception("密码错误");

        query.Result = _mapper.Map<UserDto>(user);
    }

    [EventHandler]
    public async Task AuthTypeAsync(AuthTypeQuery query)
    {
        switch (query.type)
        {
            case "Github":
                var user = await _userRepository.FindAsync(x => x.GithubId == query.id);
                query.Result = user is null ? null : _mapper.Map<UserDto>(user);
                break;
            case "Gitee":
                user = await _userRepository.FindAsync(x => x.GiteeId == query.id);
                query.Result = user is null ? null : _mapper.Map<UserDto>(user);
                break;
            default:
                throw new Exception("未知的授权类型");
        }
    }

    [EventHandler]
    public async Task GetUserAllAsync(GetUserAllQuery query)
    {
        var users = await _redisClient.GetAsync<IReadOnlyList<GetUserDto>>("allUsers") ??
                    _mapper.Map<IReadOnlyList<GetUserDto>>(await _userRepository.GetListAsync());

        query.Result = users;
    }

    [EventHandler]
    public async Task GetUserAsync(GetUserQuery query)
    {
        var user = await _userRepository.FindAsync(x => x.Id == query.userId);

        query.Result = _mapper.Map<GetUserDto>(user);
    }

    [EventHandler]
    public async Task GetEmojiAsync(GetEmojiQuery query)
    {
        var value = await _emojiRepository.GetListAsync(x => x.UserId == _userContext.GetUserId<Guid>());

        query.Result = _mapper.Map<IReadOnlyList<EmojiDto>>(value);
    }
}