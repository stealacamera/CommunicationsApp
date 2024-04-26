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

    public async Task<bool> DoesUserBelongToChannel(int userId, int channelId)
    {
        var channel = await GetByIdAsync(channelId);

        if (channel == null)
            return false;

        return channel.OwnerId == userId 
               || channel.Members.Where(e => e.Id == userId).Any();
    }
}
