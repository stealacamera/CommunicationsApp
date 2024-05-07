using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IChannelsRepository : IBaseStrongEntityRepository<Channel, int>
{
    Task<IEnumerable<Channel>> GetAllForUserAsync(int userId);
    Task<Channel?> GetByCodeAsync(string code);
}