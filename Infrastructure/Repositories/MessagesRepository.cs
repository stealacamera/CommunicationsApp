using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Infrastructure.Repositories;

internal class MessagesRepository : BaseSoftDeleteRepository<Message, int>, IMessagesRepository
{
    public MessagesRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}
