using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Operations.ChannelMembers.Queries.GetAllMembershipsForUser;
using CommunicationsApp.Application.Operations.ChannelMembers.Queries.IsUserMemberOfChannel;
using CommunicationsApp.Application.Operations.Channels.Queries.GetChannelByCode;
using CommunicationsApp.Application.Operations.Messages.Queries.GetLatestMessageForChannel;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CommunicationsApp.Web.Common.Hubs;

public sealed class ChatHub : Hub
{
    private readonly string _receiveMessageMethod = "ReceiveMessage",
                            _deleteMessageMethod = "DeleteMessage";
    
    private readonly string _joinChannelMethod = "JoinChannel",
                            _leaveChannelMethod = "LeaveChannel";
    
    private readonly string _addedToChannelMethod = "AddedToChannel",
                            _removedFromChannelMethod = "RemovedFromChannel";

    private readonly ISender _sender;

    public ChatHub(ISender sender) : base()
        => _sender = sender;

    public override async Task OnConnectedAsync()
    {
        if (Context.UserIdentifier == null)
            throw new UnauthorizedAccessException();

        GetAllMembershipsForUserCommand command = new(int.Parse(Context.UserIdentifier));
        var memberships = await _sender.Send(command);

        // If failed, user is not registered
        if (memberships.Failed)
            throw new UnauthorizedAccessException();

        foreach (var membership in memberships.Value)
            await Groups.AddToGroupAsync(Context.ConnectionId, membership.Channel.Code);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.UserIdentifier == null)
            return;

        GetAllMembershipsForUserCommand command = new(int.Parse(Context.UserIdentifier));
        var memberships = await _sender.Send(command);

        foreach (var membership in memberships.Value!)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, membership.Channel.Code);

        base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToChannel(Message message, string channelCode)
    {
        await Clients.Group(channelCode).SendAsync(_receiveMessageMethod, message, channelCode);
    }

    public async Task DeleteMessageFromChannel(string channelCode, int deletedMessageId)
    {
        await Clients.Group(channelCode).SendAsync(_deleteMessageMethod, deletedMessageId, channelCode);
    }

    public async Task JoinChannel(string channelCode)
    {
        // Check if user is member of channel
        if (Context.UserIdentifier == null)
            throw new UnauthorizedAccessException();

        GetChannelByCodeQuery channelQuery = new(channelCode);
        var channelResult = await _sender.Send(channelQuery);

        if (channelResult.Failed)
            return;

        IsUserMemberOfChannelQuery membershipQuery = new(int.Parse(Context.UserIdentifier), channelResult.Value.Id);
        var isUserMember = await _sender.Send(membershipQuery);

        if (!isUserMember)
            return;

        GetLatestChannelForMessageQuery messageQuery = new(channelResult.Value.Id);
        var messageResult = await _sender.Send(messageQuery);

        await Groups.AddToGroupAsync(Context.ConnectionId, channelCode);

        await Clients.Group(channelCode)
                     .SendAsync(
                        _joinChannelMethod,
                        new Channel_BriefOverview(channelResult.Value, messageResult.Value));
    }

    public async Task LeaveChannel(string channelCode)
    {
        GetChannelByCodeQuery channelQuery = new(channelCode);
        var channelResult = await _sender.Send(channelQuery);

        if (channelResult.Failed)
            return;

        IsUserMemberOfChannelQuery membershipQuery = new(int.Parse(Context.UserIdentifier), channelResult.Value.Id);
        var isUserMember = await _sender.Send(membershipQuery);

        if (!isUserMember)
            return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelCode);
    }

    public async Task AddMembersToChannel(IList<ChannelMember> newMembers)
    {
        string channelCode = newMembers[0].Channel.Code;

        // Alert existing members which users were added
        await Clients.Group(channelCode)
                     .SendAsync(_addedToChannelMethod, channelCode, newMembers);

        var memberIds = newMembers.Select(e => e.Member.Id.ToString())
                               .ToList()
                               .AsReadOnly();

        // Alert newly added users
        await Clients.Users(memberIds)
                     .SendAsync(_addedToChannelMethod, channelCode, newMembers);
    }

    public async Task RemoveMembersFromChannel(string channelCode, IList<int> removedMemberIds)
    {
        var userIds = removedMemberIds.Select(e => e.ToString())
                                      .ToList()
                                      .AsReadOnly();

        await Clients.Group(channelCode)
                     .SendAsync(_removedFromChannelMethod, channelCode, removedMemberIds);
    }
}