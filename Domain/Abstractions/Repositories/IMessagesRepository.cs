using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IMessagesRepository : IBaseStrongEntityRepository<Message, int>
{
    Task<CursorPaginatedEnumerable<int, Message>> GetAllForChannelAsync(
        int channelId, 
        int? cursor, 
        int pageSize, 
        CancellationToken cancellationToken);
    
    Task<Message?> GetLatestForChannelAsync(int channelId, CancellationToken cancellationToken);
}