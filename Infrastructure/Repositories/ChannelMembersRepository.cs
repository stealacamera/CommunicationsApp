using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Infrastructure.Repositories;

public sealed class ChannelMembersRepository : BaseRepository<ChannelMember>, IChannelMembersRepository
{
    public ChannelMembersRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<ChannelMember?> GetByIdsAsync(int memberId, int channelId)
        => await _set.FindAsync(memberId, channelId);
}
