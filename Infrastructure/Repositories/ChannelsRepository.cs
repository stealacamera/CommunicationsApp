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

    public async Task<Channel?> GetByIdAsync(int id, bool includeMembers = false)
    {
        if (includeMembers)
        {
            var channel = await _untrackedSet.Include(e => e.Members)
                                             .FirstOrDefaultAsync(e => e.Id == id);

            return channel;
        }
        else
            return await base.GetByIdAsync(id);
    }

    public override Task<Channel?> GetByIdAsync(int id)
    {
        throw new InvalidOperationException();
    }

    public override async Task<bool> DoesInstanceExistAsync(int id)
        => await GetByIdAsync(id, false) != null;

    public async Task<IEnumerable<Channel>> GetAllForUser(int userId)
    {
        IQueryable<Channel> query = _untrackedSet.Where(
            e => e.OwnerId == userId
            || e.Members.Where(e => e.Id == userId).Any());

        return await query.ToListAsync();
    }

    public override Task<Channel> AddAsync(Channel entity)
    {
        entity.Code = CreateUniqueCode();
        return base.AddAsync(entity);
    }

    private string CreateUniqueCode()
    {
        Guid code = Guid.NewGuid();
        string shortenedCode = Convert.ToBase64String(code.ToByteArray());

        // Removes '==' that are always at the end
        return shortenedCode.Remove(shortenedCode.Length - 2); 
    }

    public async Task<bool> DoesUserBelongToChannelAsync(int userId, int channelId)
    {
        var channel = await _untrackedSet.Include(e => e.Members)
                                         .FirstOrDefaultAsync(e => e.Id == channelId);

        if (channel == null)
            return false;

        return channel.OwnerId == userId 
               || channel.Members.Where(e => e.Id == userId).Any();
    }
}
