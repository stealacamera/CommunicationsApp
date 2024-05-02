using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Infrastructure.Repositories;

internal class MessagesRepository : BaseSoftDeleteRepository<Message, int>, IMessagesRepository
{
    public MessagesRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<IList<Message>> GetAllForChannelAsync(int channelId)
    {
        IQueryable<Message> query = _untrackedSet.Where(e => e.ChannelId == channelId);
        return await query.ToListAsync();
    }
}
