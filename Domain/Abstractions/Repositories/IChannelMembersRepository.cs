using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IChannelMembersRepository : IBaseRepository<ChannelMember>
{
    Task<ChannelMember?> GetByIdsAsync(int memberId, int channelId);
    Task<bool> IsUserMemberOfChannelAsync(int userId, int channelId);

    Task<IEnumerable<ChannelMember>> GetAllForChannelAsync(int channelId);
    Task<IEnumerable<ChannelMember>> GetAllForUserAsync(int userId);
}
