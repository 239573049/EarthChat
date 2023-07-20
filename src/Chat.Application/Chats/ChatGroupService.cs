using Chat.Chat;
using Chat.Chats.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Chat.Chats;

public class ChatGroupService : ApplicationService, IChatGroupService, IScopedDependency
{
    private readonly IRepository<ChatGroup> _chatGroupRepository;
    private readonly IRepository<ChatGroupInUser> _chatGroupInUserRepository;
    private readonly ICurrentUser _current;

    public ChatGroupService(IRepository<ChatGroup> chatGroupRepository, ICurrentUser current, IRepository<ChatGroupInUser> chatGroupInUserRepository)
    {
        _chatGroupRepository = chatGroupRepository;
        _current = current;
        _chatGroupInUserRepository = chatGroupInUserRepository;
    }

    /// <inheritdoc />
    public async Task CreateAsync(CreateChatGroupDto input)
    {
        if ((await _chatGroupRepository.CountAsync(x => x.CreatorId == _current.Id)) > 500)
        {
            throw new BusinessException(message: "超出最大限制");
        }

        await _chatGroupRepository.InsertAsync(new ChatGroup()
        {
            Name = input.Name,
            Avater = input.Avater,
            Introduce = input.Introduce
        });

    }

    /// <inheritdoc />
    public async Task UpdateAsync(ChatGroupDto input)
    {
        var data = await _chatGroupRepository.FirstOrDefaultAsync(x => x.Id == input.Id);

        if (data == null)
        {
            return;
        }

        data.Avater = input.Avater;
        data.Introduce = input.Introduce;
        data.Name = input.Name;
        await _chatGroupRepository.UpdateAsync(data);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await _chatGroupRepository.DeleteAsync(x => x.Id == id && x.CreatorId == _current.Id);
    }

    /// <inheritdoc />
    public async Task<List<ChatGroupDto>> GetListAsync()
    {
        var data = await _chatGroupRepository.GetListAsync(x => x.CreatorId == _current.Id);

        return ObjectMapper.Map<List<ChatGroup>, List<ChatGroupDto>>(data);
    }

    /// <inheritdoc />
    public async Task AddGroupChatAsync(Guid groupId, Guid userId)
    {
        var data = await _chatGroupRepository.FirstOrDefaultAsync(x => x.Id == groupId);
        if (data == null)
        {
            throw new BusinessException("群聊消失了");
        }

        if (await _chatGroupInUserRepository.AnyAsync(x => x.ChatGroupId == groupId && x.UserId == userId))
        {
            throw new BusinessException("请勿重复加入");
        }

        await _chatGroupInUserRepository.InsertAsync(new ChatGroupInUser()
        {
            UserId = userId,
            ChatGroupId = groupId
        });
    }
}