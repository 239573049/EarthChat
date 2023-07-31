namespace Chat.Contracts.Chats;

public class ChatGroupDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Avatar { get; set; }
    
    public string Description { get; set; }
}