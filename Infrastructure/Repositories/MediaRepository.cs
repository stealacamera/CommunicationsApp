using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Infrastructure.Repositories;

internal class MediaRepository : BaseStrongEntityRepository<Media, int>, IMediaRepository
{
    public MediaRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task AddAsync(Media[] entities, CancellationToken cancellationToken)
    {
        await _set.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<bool> DoesMessageHaveAnyAsync(int messageId, CancellationToken cancellationToken)
    {
        IQueryable<Media> query = _untrackedSet.Where(e => e.MessageId == messageId);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Media>> GetAllForMessage(int messageId, CancellationToken cancellationToken)
    {
        IQueryable<Media> query = _untrackedSet.Where(e => e.MessageId == messageId);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task RemoveAllForMessage(int messageId, CancellationToken cancellationToken)
    {
        IQueryable<Media> query = _untrackedSet.Where(e => e.MessageId == messageId);
        await query.ExecuteDeleteAsync(cancellationToken);
    }
}