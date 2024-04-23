using CommunicationsApp.Domain.Entities;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IMessagesRepository : IBaseStrongEntityRepository<Message, int>
{
}