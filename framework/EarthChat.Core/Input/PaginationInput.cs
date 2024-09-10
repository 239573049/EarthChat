namespace EarthChat.Core.Input;

/// <summary>
/// 分页请求
/// </summary>
public class PaginationInput : PaginationInputBase
{
    /// <summary>
    /// 关键字
    /// </summary>
    public string? Keyword { get; set; }
}