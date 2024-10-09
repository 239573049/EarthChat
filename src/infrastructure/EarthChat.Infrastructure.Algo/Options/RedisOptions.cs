namespace EarthChat.Infrastructure.Algo.Options;

public class RedisOptions
{
    public string Prefix { get; set; } = "algo";
    
    /// <summary>
    /// 连接字符串
    /// </summary>
    public string? ConnectionString { get; set; }
    
    /// <summary>
    /// 验证
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new ArgumentNullException(nameof(ConnectionString));
        }
    }
}