namespace Chat.EventsBus.Contract;

/// <summary>
/// 定义的处理程序接口
/// </summary>
/// <typeparam name="TEto"></typeparam>
public interface IEventsBusHandle<in TEto> where TEto : class
{
    Task HandleAsync(TEto eto);
}