using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Infrastructure.Repositories;

internal abstract class BaseRepository<TEntity>
    : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly DbSet<TEntity> _set;
    protected readonly IQueryable<TEntity> _untrackedSet;

    public BaseRepository(AppDbContext appDbContext)
    {
        _set = appDbContext.Set<TEntity>();
        _untrackedSet = _set.AsNoTracking();
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _set.AddAsync(entity, cancellationToken);
        return entity;
    }
}

internal abstract class BaseStrongEntityRepository<TEntity, TKey>
    : BaseRepository<TEntity>, IBaseStrongEntityRepository<TEntity, TKey>
    where TEntity : BaseStrongEntity<TKey>
{
    protected BaseStrongEntityRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public virtual async Task<bool> DoesInstanceExistAsync(TKey id, CancellationToken cancellationToken)
        => await GetByIdAsync(id, cancellationToken) != null;

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
    {
        return await _set.FindAsync(id, cancellationToken);
    }
}

internal class BaseSoftDeleteRepository<TEntity, TKey>
    : BaseStrongEntityRepository<TEntity, TKey>
    where TEntity : BaseSoftDeleteEntity<TKey>
{
    public BaseSoftDeleteRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public override async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
    {
        var entity = await base.GetByIdAsync(id, cancellationToken);

        if (entity != null && entity.DeletedAt == null)
            return entity;

        return null;
    }
}