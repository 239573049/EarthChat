namespace Chat.EventsBus.Contract;

public class EventsBusOptions
{
    /// <summary>
    /// 重试次数
    /// </summary>
    public readonly int Retry = 3;
}