using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
}

public interface IBaseStrongEntityRepository<TEntity, TKey>
    : IBaseRepository<TEntity>
    where TEntity : BaseStrongEntity<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken);
    Task<bool> DoesInstanceExistAsync(TKey id, CancellationToken cancellationToken);
}