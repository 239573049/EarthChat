using AutoMapper;
using Chat.Contracts.Chats;
using Chat.Contracts.Hubs;
using Chat.Service.Application.Hubs.Commands;
using Chat.Service.Application.Users.Commands;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;
using Chat.Service.Infrastructure.Repositories;
using Masa.BuildingBlocks.Data.UoW;

namespace Chat.Service.Application.Users;

public class UserCommandHandler
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IEventBus _eventBus;
    private readonly IChatGroupInUserRepository _chatGroupInUserRepository;
    private readonly IChatGroupRepository _chatGroupRepository;
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly IEmojiRepository _emojiRepository;

    public UserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper,
        IChatGroupInUserRepository chatGroupInUserRepository, IChatGroupRepository chatGroupRepository,
        IUserContext userContext, IEmojiRepository emojiRepository, IEventBus eventBus, IFriendRequestRepository friendRequestRepository)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _chatGroupInUserRepository = chatGroupInUserRepository;
        _chatGroupRepository = chatGroupRepository;
        _userContext = userContext;
        _emojiRepository = emojiRepository;
        _eventBus = eventBus;
        _friendRequestRepository = friendRequestRepository;
    }

    [EventHandler(1)]
    public async Task CreateUserAsync(CreateUserCommand command)
    {
        if (await _userRepository.GetCountAsync(x => x.Account == command.CreateUserDto.Account) > 0)
            throw new Exception("用户已存在");

        var user = new User(command.CreateUserDto.Account, command.CreateUserDto.Password, command.CreateUserDto.Avatar,
            command.CreateUserDto.Name)
        {
            GiteeId = command.CreateUserDto.GiteeId,
            GithubId = command.CreateUserDto.GithubId
        };

        await _userRepository.AddAsync(user);
        command.Result = _mapper.Map<UserDto>(user);
    }

    [EventHandler(2)]
    public async Task DefaultGroupUserAsync(CreateUserCommand command)
    {
        var defaultGroup = await _chatGroupRepository.GetListAsync(x => x.Default);

        // TODO: 默认加入的群组
        await _chatGroupInUserRepository.AddRangeAsync(defaultGroup.Select(x => new ChatGroupInUser()
        {
            UserId = command.Result.Id,
            ChatGroupId = x.Id
        }));


        var systemCommand = new SystemCommand(new Notification()
        {
            createdTime = DateTime.Now,
            type = NotificationType.GroupAppendUser,
            content = "新增用户",
        }, defaultGroup.Select(x => x.Id).ToArray(), true);

        await _eventBus.PublishAsync(systemCommand);
    }

    [EventHandler]
    public async Task AuthUpdateAsync(AuthUpdateCommand command)
    {
        var user = await _userRepository.FindAsync(x => x.Id == command.userId);
        if (user == null)
        {
            return;
        }

        user.Name = command.name;
        user.Avatar = command.avatar;

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateUserCommand command)
    {
        var user = await _userRepository.FindAsync(x => x.Id == _userContext.GetUserId<Guid>());
        if (user == null)
        {
            return;
        }

        user.Name = command.Dto.Name;
        user.Avatar = command.Dto.Avatar;

        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task AddEmojiAsync(AddEmojiCommand command)
    {
        if (await _emojiRepository.GetCountAsync(x => x.UserId == _userContext.GetUserId<Guid>()) > 500)
        {
            throw new UserFriendlyException("您的表情包超出最大限额。");
        }

        if (await _emojiRepository.GetCountAsync(x =>
                x.UserId == _userContext.GetUserId<Guid>() && x.Path == command.path) > 0)
        {
            throw new UserFriendlyException("已经存在表情包");
        }

        var emoji = new Emoji()
        {
            Path = command.path,
            Sort = 1,
            UserId = _userContext.GetUserId<Guid>(),
            CreationTime = DateTime.Now
        };

        await _emojiRepository.AddAsync(emoji);
    }

    [EventHandler]
    public async Task DeleteEmojiAsync(DeleteEmojiCommand command)
    {
        await _emojiRepository.RemoveAsync(command.Id);
    }

    [EventHandler]
    public async Task FriendRegistrationAsync(FriendRegistrationCommand command)
    {
        var query = new FriendStateQuery(command.Input.BeAppliedForId);
        await _eventBus.PublishAsync(query);

        if (query.Result)
        {
            throw new UserFriendlyException("已经存在好友关系");
        }

        var value = await _friendRequestRepository
            .FindAsync(x =>
                x.RequestId == _userContext.GetUserId<Guid>() && x.BeAppliedForId == command.Input.BeAppliedForId);

        if (value?.State == FriendState.ApplyFor)
        {
            throw new UserFriendlyException("请勿重复发起申请");
        }

        var request = new FriendRequest()
        {
            RequestId = _userContext.GetUserId<Guid>(),
            ApplicationDate = DateTime.Now,
            BeAppliedForId = command.Input.BeAppliedForId,
            Description = command.Input.Description,
            State = FriendState.ApplyFor
        };

        await _friendRequestRepository.AddAsync(request);

        var systemCommand = new SystemCommand(new Notification()
        {
            content = "有新的好友申请",
            createdTime = DateTime.Now,
            type = NotificationType.FriendRequest
        }, new[] { command.Input.BeAppliedForId }, false);

        // 发送好友系统通知
        await _eventBus.PublishAsync(systemCommand);
    }
}