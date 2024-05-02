using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IChannelsRepository : IBaseStrongEntityRepository<Channel, int>
{
    Task<IEnumerable<Channel>> GetAllForUser(int userId);
    Task<Channel?> GetByIdAsync(int id, bool includeMembers = false);
    Task<bool> DoesUserBelongToChannelAsync(int userId, int channelId);
}