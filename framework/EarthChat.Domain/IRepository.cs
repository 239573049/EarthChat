using System.Linq.Expressions;

namespace EarthChat.Domain;

public interface IRepository<TEntity>
{
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> InsertAsync(TEntity entity);

    /// <summary>
    /// 批量添加数据
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    Task InsertAsync(IEnumerable<TEntity> entities);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task DeleteAsync(TEntity entity);

    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

    Task<List<TEntity>> GetListAsync();

    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    Task<int> SaveChangesAsync();

    Task<(int total, IEnumerable<TEntity> result)> GetPageListAsync(Expression<Func<TEntity, bool>> predicate,
        int pageIndex,
        int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAsc = true);
}