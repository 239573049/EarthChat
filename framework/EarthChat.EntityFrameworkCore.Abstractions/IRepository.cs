using System.Linq.Expressions;

namespace EarthChat.EntityFrameworkCore.Repositories;

/// <summary>
/// 定义通用仓储接口，提供基本的CRUD操作和查询功能。
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// 异步插入一个实体。
    /// </summary>
    /// <param name="entity">要插入的实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>插入的实体</returns>
    Task<TEntity> InsertAsync(TEntity? entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步更新一个实体。
    /// </summary>
    /// <param name="entity">要更新的实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新后的实体</returns>
    Task<TEntity?> UpdateAsync(TEntity? entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步删除一个实体。
    /// </summary>
    /// <param name="entity">要删除的实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task DeleteAsync(TEntity? entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件异步删除实体。
    /// </summary>
    /// <param name="predicate">删除条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除的实体数量</returns>
    Task<int> DeleteAsync(Expression<Func<TEntity?, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件异步查找一个实体。
    /// </summary>
    /// <param name="predicate">查找条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>找到的实体</returns>
    Task<TEntity?> FindAsync(Expression<Func<TEntity?, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件异步获取实体列表。
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体列表</returns>
    Task<List<TEntity?>> GetListAsync(Expression<Func<TEntity?, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件异步统计实体数量。
    /// </summary>
    /// <param name="predicate">统计条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体数量</returns>
    Task<int> CountAsync(Expression<Func<TEntity?, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件异步判断是否存在实体。
    /// </summary>
    /// <param name="predicate">判断条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在实体</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity?, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件异步获取第一个或默认的实体。
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>第一个或默认的实体</returns>
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity?, bool>> predicate, CancellationToken cancellationToken = default);
}