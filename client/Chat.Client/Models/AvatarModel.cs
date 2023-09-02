namespace Chat.Client.Models;

public class AvatarModel
{
    public string Id { get; set; }

    public byte[] Bytes { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; }
}