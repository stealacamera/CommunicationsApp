using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using CommunicationsApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CommunicationsApp.Infrastructure;

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

public class WorkUnit : IWorkUnit
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public WorkUnit(IServiceProvider serviceProvider)
    {
        _dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    private IUsersRepository _usersRepository;
    public IUsersRepository UsersRepository
    {
        get
        {
            _usersRepository ??= new UsersRepository(_userManager);
            return _usersRepository;
        }
    }

    private IMessagesRepository _messagesRepository;
    public IMessagesRepository MessagesRepository
    {
        get
        {
            _messagesRepository ??= new MessagesRepository(_dbContext);
            return _messagesRepository;
        }
    }

    private IChannelsRepository _channelsRepository;
    public IChannelsRepository ChannelsRepository
    {
        get
        {
            _channelsRepository ??= new ChannelsRepository(_dbContext);
            return _channelsRepository;
        }
    }

    private IChannelMembersRepository _channelMembersRepository;
    public IChannelMembersRepository ChannelMembersRepository
    {
        get
        {
            _channelMembersRepository ??= new ChannelMembersRepository(_dbContext);
            return _channelMembersRepository;
        }
    }
}
