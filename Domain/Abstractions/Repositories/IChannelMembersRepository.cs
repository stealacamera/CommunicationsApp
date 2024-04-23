using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IChannelMembersRepository : IBaseRepository<ChannelMember>
{
    Task<ChannelMember?> GetByIdsAsync(int memberId, int channelId);
}
