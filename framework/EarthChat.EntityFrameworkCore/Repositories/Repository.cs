using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EarthChat.EntityFrameworkCore.Repositories;

public abstract class Repository<TDbContext, TEntity>(TDbContext dbContext) : IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    protected readonly TDbContext DbContext = dbContext;

    protected readonly DbSet<TEntity?> DbSet = dbContext.Set<TEntity>();

    public async Task<TEntity> InsertAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var entityEntry = await DbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return entityEntry.Entity;
    }

    public async Task<TEntity?> UpdateAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);

        return await Task.FromResult(entity);
    }

    public async Task DeleteAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);

        await Task.CompletedTask;
    }

    public async Task<int> DeleteAsync(Expression<Func<TEntity?, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity?, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<List<TEntity?>> GetListAsync(Expression<Func<TEntity?, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity?, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity?, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity?, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }
}