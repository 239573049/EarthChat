using System;

namespace Chat.Client.Models;

public class Users
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string Description { get; set; }
    
}