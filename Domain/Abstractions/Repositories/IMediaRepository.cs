using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IMediaRepository : IBaseStrongEntityRepository<Media, int>
{
    Task AddAsync(Media[] entities);
    Task<IEnumerable<Media>> GetAllForMessage(int messageId);
    Task RemoveAllForMessage(int messageId);
}
