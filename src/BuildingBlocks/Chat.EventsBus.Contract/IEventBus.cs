namespace Chat.EventsBus.Contract;

public interface IEventBus
{
    /// <summary>
    /// 发布事件
    /// </summary>
    /// <param name="eto"></param>
    /// <typeparam name="TEto"></typeparam>
    /// <returns></returns>
    Task PushAsync<TEto>(TEto eto) where TEto : class;
}