using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IMessagesRepository : IBaseStrongEntityRepository<Message, int>
{
    Task<IList<Message>> GetAllForChannelAsync(int channelId);
    Task<Message?> GetLatestForChannelAsync(int channelId);
}