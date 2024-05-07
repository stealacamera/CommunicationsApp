using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Infrastructure.Repositories;

internal sealed class ChannelMembersRepository : BaseRepository<ChannelMember>, IChannelMembersRepository
{
    public ChannelMembersRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<IEnumerable<ChannelMember>> GetAllForChannelAsync(int channelId)
    {
        IQueryable<ChannelMember> query = _untrackedSet.Where(e => e.ChannelId == channelId);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<ChannelMember>> GetAllForUserAsync(int userId)
    {
        IQueryable<ChannelMember> query = _untrackedSet.Where(e => e.MemberId == userId);
        return await query.ToListAsync();
    }

    public async Task<ChannelMember?> GetByIdsAsync(int memberId, int channelId)
        => await _set.FindAsync(memberId, channelId);

    public async Task<bool> IsUserMemberOfChannelAsync(int userId, int channelId)
        => (await _set.FindAsync(userId, channelId)) != null;
}
