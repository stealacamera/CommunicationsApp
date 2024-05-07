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

    public override async Task<bool> DoesInstanceExistAsync(int id)
        => await GetByIdAsync(id) != null;

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

    public async Task<Channel?> GetByCodeAsync(string code)
    {
        IQueryable<Channel> query = _untrackedSet.Where(e => e.Code == code);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Channel>> GetAllForUserAsync(int userId)
    {
        IQueryable<Channel> query = _untrackedSet.Where(
            e => e.Members.Where(e => e.Id == userId).Any());

        return await query.ToListAsync();
    }
}
