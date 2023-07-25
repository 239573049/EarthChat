namespace Chat.Service.Options;

public class JwtOptions
{
    /// <summary>
    ///     秘钥
    /// </summary>
    public string Secret { get; set; }

    /// <summary>
    ///     有效时间（小时）
    /// </summary>
    public int EffectiveHours { get; set; }
}