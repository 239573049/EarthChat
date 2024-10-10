using System.Linq.Expressions;

namespace EarthChat.EntityFrameworkCore.Repositories;

public static class QueryableExtensions
{
    /// <summary>
    /// 条件查询
    /// </summary>
    /// <param name="query">
    ///    查询对象
    /// </param>
    /// <param name="condition">
    ///   条件
    /// </param>
    /// <param name="predicate">
    ///  查询条件
    /// </param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> WhereIf<TEntity>(this IQueryable<TEntity> query, bool condition,
        Expression<Func<TEntity, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="query">
    ///    查询对象
    /// </param>
    /// <param name="page">
    ///   页码
    /// </param>
    /// <param name="pageSize">
    ///  每页数量
    /// </param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> PageBy<TEntity>(this IQueryable<TEntity> query, int page, int pageSize)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}