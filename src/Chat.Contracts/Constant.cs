namespace Chat.Contracts;

public class Constant
{
    public const string ChatGPT = "ChatGPT";
    
    public const string Github = "Github";
    
    public const string Success = "200";
    
    public const string GetUserGroup  = "GetUserGroup";

    public const string OnLineKey = "OnLineKey:";

    public const string Connections = "Connections:";
    public class Role
    {
        public const string Default = "Chat:Role";
    }
    
    public class Group
    {
        private const string Default = "Chat:Group";

        /// <summary>
        /// 群聊人数Key
        /// </summary>
        public const string GroupUsers = Default + ":Users";
    }
}