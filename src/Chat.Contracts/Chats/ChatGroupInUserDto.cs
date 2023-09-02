using Chat.Contracts.Users;

namespace Chat.Contracts.Chats;

public class ChatGroupInUserDto
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid ChatGroupId { get; set; }
    
    public ChatGroupDto ChatGroup { get; set; }

    public UserDto User { get; set; }   
}