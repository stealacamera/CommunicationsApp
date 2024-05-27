using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IChannelMembersRepository : IBaseRepository<ChannelMember>
{
    void Remove(ChannelMember member);

    Task<ChannelMember?> GetByIdsAsync(int memberId, int channelId, CancellationToken cancellationToken = default);
    Task<bool> IsUserMemberOfChannelAsync(int userId, int channelId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ChannelMember>> GetAllForChannelAsync(int channelId, CancellationToken cancellationToken = default);
    Task<int> GetCountForChannelAsync(int channelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChannelMember>> GetAllForUserAsync(int userId, CancellationToken cancellationToken = default);
}
