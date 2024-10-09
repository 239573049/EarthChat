namespace EarthChat.Ddd;

/// <summary>
/// 扩展属性接口
/// </summary>
public interface IExtend
{
    IDictionary<string, string> Extend { get; set; }
}