namespace EarthChat.Core.Input;

/// <summary>
///     分页请求基类
/// </summary>
public abstract class PaginationInputBase
{
    private int _pageSize = 10;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value switch
            {
                <= 10 => 10,
                >= 1000 => 1000,
                _ => value
            };
        }
    }
}