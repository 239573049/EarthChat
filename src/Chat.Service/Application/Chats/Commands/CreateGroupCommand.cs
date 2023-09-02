using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Commands;

/// <summary>
/// 创建群组。
/// </summary>
/// <param name="name"></param>
/// <param name="avatar"></param>
/// <param name="description"></param>
public record CreateGroupCommand(CreateGroupDto Dto) : Command;