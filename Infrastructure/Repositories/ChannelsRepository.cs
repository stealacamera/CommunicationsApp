using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Infrastructure.Repositories;

internal class ChannelsRepository
    : BaseSoftDeleteRepository<Channel, int>, IChannelsRepository
{
    public ChannelsRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public override Task<Channel> AddAsync(Channel entity, CancellationToken cancellationToken)
    {
        entity.Code = CreateUniqueCode();
        return base.AddAsync(entity, cancellationToken);
    }

    private string CreateUniqueCode()
    {
        Guid code = Guid.NewGuid();
        string shortenedCode = Convert.ToBase64String(code.ToByteArray());

        // Removes '==' that are always at the end
        return shortenedCode.Remove(shortenedCode.Length - 2); 
    }

    public async Task<Channel?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        IQueryable<Channel> query = _untrackedSet.Where(e => e.Code == code);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Channel>> GetAllForUserAsync(int userId, CancellationToken cancellationToken)
    {
        IQueryable<Channel> query = _untrackedSet.Where(
            e => e.Members.Where(e => e.Id == userId).Any());

        return await query.ToListAsync(cancellationToken);
    }
}
