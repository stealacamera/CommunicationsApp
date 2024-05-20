using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Infrastructure.Repositories;

internal class MessagesRepository : BaseSoftDeleteRepository<Message, int>, IMessagesRepository
{
    public MessagesRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<CursorPaginatedEnumerable<int, Message>> GetAllForChannelAsync(
        int channelId, 
        int? cursor, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        IQueryable<Message> query = _untrackedSet.Where(e => e.ChannelId == channelId);
        query = query.OrderByDescending(e => e.Id);

        return await CursorPaginatedEnumerable<int, Message>.CreateAsync(
            cursor, pageSize, 
            nameof(Message.Id), 
            query, 
            cancellationToken, 
            getOlderValues: false);
    }

    public async Task<Message?> GetLatestForChannelAsync(int channelId, CancellationToken cancellationToken)
    {
        IQueryable<Message> query = _untrackedSet.Where(e => e.ChannelId == channelId);
        query = query.OrderByDescending(e => e.Id);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}
