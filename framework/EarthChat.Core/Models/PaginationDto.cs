namespace EarthChat.Core.Models;

/// <summary>
/// 分页模型
/// </summary>
/// <typeparam name="TResult"></typeparam>
public class PaginationDto<TResult> where TResult : class
{
    public long Total { get; set; }

    public IReadOnlyList<TResult> Items { get; set; }

    public static PaginationDto<TResult> Create(long total, IReadOnlyList<TResult> items)
    {
        return new PaginationDto<TResult>
        {
            Total = total,
            Items = items
        };
    }
}