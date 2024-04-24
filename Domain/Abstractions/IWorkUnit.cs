using CommunicationsApp.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CommunicationsApp.Domain.Abstractions;

public interface IWorkUnit
{
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task SaveChangesAsync();

    #region Repositories
    IUsersRepository UsersRepository { get; }
    IMessagesRepository MessagesRepository { get; }
    IChannelsRepository ChannelsRepository { get; }
    IChannelMembersRepository ChannelMembersRepository { get; }
    #endregion
}