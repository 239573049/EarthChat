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
    private readonly IChatGroupInUserRepository _chatGroupInUserRepository;

    public QueryHandler(IChatMessageRepository chatMessageRepository, IMapper mapper,
        IChatGroupInUserRepository chatGroupInUserRepository)
    {
        _chatMessageRepository = chatMessageRepository;
        _mapper = mapper;
        _chatGroupInUserRepository = chatGroupInUserRepository;
    }

    [EventHandler]
    public async Task GetListAsync(GeChatMessageListQuery query)
    {
        var list = await _chatMessageRepository.GetListAsync(query.page, query.pageSize);

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
            Result = _mapper.Map<List<ChatMessageDto>>(list.OrderBy(x => x.CreationTime)),
            Total = await _chatMessageRepository.GetCountAsync()
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
            _mapper.Map<List<ChatGroupInUserDto>>(result);

        query.Result.Add(new ChatGroupInUserDto
        {
            Id = Guid.Empty,
            ChatGroupId = query.groupId,
            UserId = Guid.Empty,
            User = new UserDto()
            {
                Id = Guid.Empty,
                Account = string.Empty,
                Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png",
                Name = "聊天机器人",
            }
        });
    }
}