namespace Chat.EventsBus.Contract;

public class BaseEto<T>
{
    /// <summary>
    /// 事件id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Eto数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 扩展参数
    /// </summary>
    public Dictionary<string,string> Extend { get; set; }
}