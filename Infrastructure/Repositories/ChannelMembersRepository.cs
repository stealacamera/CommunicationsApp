using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Infrastructure.Repositories;

internal sealed class ChannelMembersRepository : BaseRepository<ChannelMember>, IChannelMembersRepository
{
    public ChannelMembersRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<IEnumerable<ChannelMember>> GetAllForChannelAsync(int channelId, CancellationToken cancellationToken)
    {
        IQueryable<ChannelMember> query = _untrackedSet.Where(e => e.ChannelId == channelId);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChannelMember>> GetAllForUserAsync(int userId, CancellationToken cancellationToken)
    {
        IQueryable<ChannelMember> query = _untrackedSet.Where(e => e.MemberId == userId);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ChannelMember?> GetByIdsAsync(int memberId, int channelId, CancellationToken cancellationToken)
        => await _set.FindAsync(memberId, channelId, cancellationToken);

    public async Task<int> GetCountForChannelAsync(int channelId, CancellationToken cancellationToken)
    {
        IQueryable<ChannelMember> query = _untrackedSet.Where(e => e.ChannelId == channelId);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> IsUserMemberOfChannelAsync(int userId, int channelId, CancellationToken cancellationToken)
        => (await _set.FindAsync(userId, channelId, cancellationToken)) != null;

    public void Remove(ChannelMember member)
        => _set.Remove(member);
}
