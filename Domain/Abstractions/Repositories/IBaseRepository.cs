using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
}

public interface IBaseStrongEntityRepository<TEntity, TKey>
    : IBaseRepository<TEntity>
    where TEntity : BaseStrongEntity<TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<bool> DoesInstanceExistAsync(TKey id);
}