namespace Chat.Contracts.Chats;

public class CreateGroupDto 
{
    public string Name { get; set; }

    public string Avatar { get; set; } = "assets://avatar.png";

    public string Description { get; set; }
    
}