namespace Chat.Contracts.third_party;

public class GetObtainingIPHomeDto
{
    /// <summary>
    /// 当前公网ip
    /// </summary>
    public string ip { get; set; }

    /// <summary>
    /// 省
    /// </summary>
    public string pro { get; set; }

    /// <summary>
    /// 省 code
    /// </summary>
    public string proCode { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string city { get; set; }

    /// <summary>
    /// 城市代码
    /// </summary>
    public string cityCode { get; set; }
}
