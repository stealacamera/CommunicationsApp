using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IChannelsRepository : IBaseStrongEntityRepository<Channel, int>
{
    Task<IEnumerable<Channel>> GetAllForUser(int userId);
    Task<bool> DoesUserBelongToChannel(int userId, int channelId);
}