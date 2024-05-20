using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IMediaRepository : IBaseStrongEntityRepository<Media, int>
{
    Task AddAsync(Media[] entities, CancellationToken cancellationToken);
    Task<IEnumerable<Media>> GetAllForMessage(int messageId, CancellationToken cancellationToken);
    Task<bool> DoesMessageHaveAnyAsync(int messageId, CancellationToken cancellationToken);
    Task RemoveAllForMessage(int messageId, CancellationToken cancellationToken);
}
