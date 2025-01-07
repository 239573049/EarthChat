using System.Linq.Expressions;

namespace EarthChat.Domain;

/// <summary>
/// 通用仓储接口
/// </summary>
/// <typeparam name="TDbContext">数据库上下文类型</typeparam>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IRepository<TDbContext, TEntity> : IRepository<TEntity>
{
    
}

/// <summary>
/// 通用仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IRepository<TEntity>
{
    /// <summary>
    /// 获取符合条件的实体列表
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>实体列表</returns>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 插入实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>插入的实体</returns>
    Task<TEntity> InsertAsync(TEntity entity);

    /// <summary>
    /// 批量添加数据
    /// </summary>
    /// <param name="entities">实体集合</param>
    /// <returns></returns>
    Task InsertAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns>更新的实体</returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <returns></returns>
    Task DeleteAsync(TEntity entity);

    /// <summary>
    /// 获取符合条件的第一个实体或默认值
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>实体对象或默认值</returns>
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 获取符合条件的第一个实体
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>实体对象</returns>
    Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 获取符合条件的唯一实体或默认值
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>实体对象或默认值</returns>
    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 获取符合条件的唯一实体
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>实体对象</returns>
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 获取符合条件的实体数量
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>实体数量</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 判断是否存在符合条件的实体
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>是否存在</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 获取符合条件的实体
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>实体对象</returns>
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 获取所有实体列表
    /// </summary>
    /// <returns>实体列表</returns>
    Task<List<TEntity>> GetListAsync();

    /// <summary>
    /// 删除符合条件的实体
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// 保存更改
    /// </summary>
    /// <returns>受影响的行数</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// 获取分页列表
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="orderBy">排序字段</param>
    /// <param name="isAsc">是否升序</param>
    /// <returns>分页结果</returns>
    Task<(int total, IEnumerable<TEntity> result)> GetPageListAsync(Expression<Func<TEntity, bool>> predicate,
        int pageIndex,
        int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAsc = true);
}