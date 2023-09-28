using System.Linq.Expressions;
using Chat.Service.Domain.Repositories;
using Masa.BuildingBlocks.Data;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;

namespace Chat.Service.Infrastructure.Repositories;

public abstract class
    BaseRepository<TDbContext, TEntity, TKey> : Repository<TDbContext, TEntity, TKey>, IBaseRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : IComparable where TDbContext : DbContext, IMasaDbContext
{
    protected BaseRepository(TDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return Context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }
}