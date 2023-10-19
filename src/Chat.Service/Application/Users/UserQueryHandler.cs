using AutoMapper;
using Chat.Service.Application.Users.Commands;
using Chat.Service.Domain.Users.Repositories;
using Chat.Service.Infrastructure.Helper;
using Microsoft.Extensions.Primitives;

namespace Chat.Service.Application.Users;

public class UserQueryHandler
{
    private readonly IMapper _mapper;
    private readonly IEventBus _eventBus;
    private readonly RedisClient _redisClient;
    private readonly IUserContext _userContext;
    private readonly IEmojiRepository _emojiRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFriendRepository _friendRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFriendRequestRepository _friendRequestRepository;

    public UserQueryHandler(IUserRepository userRepository, IMapper mapper, RedisClient redisClient,
        IEmojiRepository emojiRepository, IUserContext userContext, IFriendRepository friendRepository,
        IEventBus eventBus, IFriendRequestRepository friendRequestRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _redisClient = redisClient;
        _emojiRepository = emojiRepository;
        _userContext = userContext;
        _friendRepository = friendRepository;
        _eventBus = eventBus;
        _friendRequestRepository = friendRequestRepository;
        _httpContextAccessor = httpContextAccessor;
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

        var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        if ((_httpContextAccessor?.HttpContext?.Request?.Headers)?.TryGetValue("X-Forwarded-For", out var header) == true)
        {
            ip = header.ToString();
        }

        if (user?.Ip != ip)
        {
            await _eventBus.PublishAsync(new UpdateLocationCommand(user.Id, ip));
        }

        query.Result = _mapper.Map<GetUserDto>(user);
    }

    [EventHandler]
    public async Task GetEmojiAsync(GetEmojiQuery query)
    {
        var value = await _emojiRepository.GetListAsync(x => x.UserId == _userContext.GetUserId<Guid>());

        query.Result = _mapper.Map<IReadOnlyList<EmojiDto>>(value.OrderBy(x => x.Sort));
    }

    [EventHandler]
    public async Task GetUserListAsync(GetUserListQuery query)
    {
        var result = await _userRepository.GetListAsync(x => query.UserIds.Contains(x.Id));

        query.Result = _mapper.Map<List<UserDto>>(result);
    }

    [EventHandler]
    public async Task FriendStateAsync(FriendStateQuery query)
    {
        query.Result = await _friendRepository.ExistAsync(x =>
            x.SelfId == _userContext.GetUserId<Guid>() && x.FriendId == query.friendId);
    }

    [EventHandler]
    public async Task GetFriendRequestListAsync(GetFriendRequestListQuery query)
    {
        var result =
            await _friendRequestRepository.GetListAsync(_userContext.GetUserId<Guid>(), query.Page, query.PageSize);

        var count = await _friendRequestRepository.GetCountAsync(_userContext.GetUserId<Guid>());

        query.Result = new PaginatedListBase<FriendRequestDto>()
        {
            Result = _mapper.Map<List<FriendRequestDto>>(result),
            Total = count
        };
    }
}