using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Commands;

/// <summary>
/// 创建群组。
/// </summary>
public record CreateGroupCommand(CreateGroupDto Dto,string connections) : Command;