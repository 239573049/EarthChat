using AutoMapper;
using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Queries;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Application.Chats;

public class QueryHandler
{
    private readonly IMapper _mapper;
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IChatGroupRepository _chatGroupRepository;
    private readonly IChatGroupInUserRepository _chatGroupInUserRepository;

    public QueryHandler(IChatMessageRepository chatMessageRepository, IMapper mapper,
        IChatGroupInUserRepository chatGroupInUserRepository, IChatGroupRepository chatGroupRepository)
    {
        _chatMessageRepository = chatMessageRepository;
        _mapper = mapper;
        _chatGroupInUserRepository = chatGroupInUserRepository;
        _chatGroupRepository = chatGroupRepository;
    }

    [EventHandler]
    public async Task GetListAsync(GeChatMessageListQuery query)
    {
        var list = await _chatMessageRepository.GetListAsync(query.groupId,query.page, query.pageSize);

        foreach (var message in list.Where(message => message.UserId == Guid.Empty))
        {
            message.User = new User(Guid.Empty)
            {
                Account = string.Empty,
                Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png",
                Name = "聊天机器人",
            };
        }

        query.Result = new PaginatedListBase<ChatMessageDto>
        {
            Result = _mapper.Map<List<ChatMessageDto>>(list.OrderBy(x => x.CreationTime))
        };
    }

    [EventHandler]
    public async Task GetUserGroupAsync(GetUserGroupQuery query)
    {
        var ids = await _chatGroupInUserRepository.GetUserChatGroupAsync(query.userId);

        query.Result = _mapper.Map<List<ChatGroupDto>>(ids);
    }

    [EventHandler]
    public async Task GetGroupInUserAsync(GetGroupInUserQuery query)
    {
        var result = await _chatGroupInUserRepository.GetGroupInUserAsync(query.groupId);

        query.Result =
            _mapper.Map<List<UserDto>>(result);

        query.Result.Add(new UserDto()
        {
            Id = Guid.Empty,
            Account = string.Empty,
            Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png",
            Name = "聊天机器人",
        });
    }

    [EventHandler]
    public async Task GetGroupAsync(GetGroupQuery query)
    {
        var value = await _chatGroupRepository.FindAsync(x => x.Id == query.Id);

        query.Result = _mapper.Map<ChatGroupDto>(value);
    }
}