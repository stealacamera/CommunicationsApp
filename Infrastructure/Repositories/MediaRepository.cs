using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Infrastructure.Repositories;

internal class MediaRepository : BaseStrongEntityRepository<Media, int>, IMediaRepository
{
    public MediaRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task AddAsync(Media[] entities)
    {
        await _set.AddRangeAsync(entities);
    }

    public async Task<IEnumerable<Media>> GetAllForMessage(int messageId)
    {
        IQueryable<Media> query = _untrackedSet.Where(e => e.MessageId == messageId);
        return await query.ToListAsync();
    }

    public async Task RemoveAllForMessage(int messageId)
    {
        IQueryable<Media> query = _untrackedSet.Where(e => e.MessageId == messageId);
        await query.ExecuteDeleteAsync();
    }
}