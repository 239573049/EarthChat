namespace Chat.Contracts;

/// <summary>
/// 系统服务常量
/// </summary>
public class Constant
{
    public const string ChatGPT = "ChatGPT";

    public const string Github = "Github";

    public const string Success = "200";

    public const string GetUserGroup = "GetUserGroup";

    public const string OnLineKey = "OnLineKey:";

    public const string Connections = "Connections:";

    public const string Default = "Chat:";

    /// <summary>
    /// 角色类
    /// </summary>
    public class Role
    {
        public const string Default = Constant.Default + "Role";
    }

    /// <summary>
    /// 群聊类
    /// </summary>
    public class Group
    {
        private const string Default = Constant.Default + "Group";

        /// <summary>
        /// 群聊人数Key
        /// </summary>
        public const string GroupUsers = Default + ":Users";

        /// <summary>
        /// 智能助手id
        /// </summary>
        public static Guid AssistantId = Guid.Parse("6D53F694-4221-4E87-B8B2-2F54E8929303");
    }

    /// <summary>
    /// 文件类
    /// </summary>
    public class File
    {
        /// <summary>
        /// 最大上传文件数量
        /// </summary>
        public const int MaxUploadFile = 500;
    }

    /// <summary>
    /// 用户类
    /// </summary>
    public class User
    {
        private const string Default = Constant.Default + "User";

        public const string DefaultAvatar = "/favicon.png";
    }
}