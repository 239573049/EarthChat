namespace Chat.Options;

public class MinIOOptions
{
    /// <summary>
    /// 端点 <ip:port>
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    public string AccessKey { get; set; }

    /// <summary>
    /// 账号密钥
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    /// 启用/禁用HTTPS支持的布尔值(default=true)
    /// </summary>
    public bool Secure { get; set; }

    /// <summary>
    /// 存储桶
    /// </summary>
    public string Bucket { get; set; }
}