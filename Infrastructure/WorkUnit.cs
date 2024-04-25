using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using CommunicationsApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CommunicationsApp.Infrastructure;

public class WorkUnit : IWorkUnit
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public WorkUnit(IServiceProvider serviceProvider)
    {
        _dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        _signInManager = serviceProvider.GetRequiredService<SignInManager<User>>();
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
            _usersRepository ??= new UsersRepository(_userManager, _signInManager);
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
