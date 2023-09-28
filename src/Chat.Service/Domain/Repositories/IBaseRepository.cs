using System.Linq.Expressions;

namespace Chat.Service.Domain.Repositories;

public interface IBaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : IComparable
{
    Task<bool> ExistAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}