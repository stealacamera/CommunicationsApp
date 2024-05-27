using CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.GetAllMembershipsForChannel;
using CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.GetAllMembershipsForUser;
using CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.IsUserMemberOfChannel;
using CommunicationsApp.Application.Behaviour.Operations.Channels.Queries.GetChannelByCode;
using CommunicationsApp.Application.Behaviour.Operations.Messages.Queries.GetLatestMessageForChannel;
using CommunicationsApp.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.InteropServices;

namespace CommunicationsApp.Web.Common.Hubs;

public sealed class ChatHub : Hub
{
    private readonly string _receiveMessageMethod = "ReceiveMessage",
                            _deleteMessageMethod = "DeleteMessage";
    
    private readonly string _joinChannelMethod = "JoinChannel",
                            _leaveChannelMethod = "LeaveChannel";
    
    private readonly string _addedToChannelMethod = "AddedToChannel",
                            _removedFromChannelMethod = "RemovedFromChannel";

    private readonly string _onFailure = "FunctionFailed";

    private readonly ISender _sender;

    public ChatHub(ISender sender) : base()
        => _sender = sender;

    public override async Task OnConnectedAsync()
    {
        if (Context.UserIdentifier == null)
            throw new UnauthorizedAccessException();

        GetAllMembershipsForUserQuery command = new(int.Parse(Context.UserIdentifier));
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

        GetAllMembershipsForUserQuery command = new(int.Parse(Context.UserIdentifier));
        var memberships = await _sender.Send(command);

        foreach (var membership in memberships.Value!)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, membership.Channel.Code);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToChannel(Message message, string channelCode)
    {
        await Clients.Group(channelCode).SendAsync(_receiveMessageMethod, message, channelCode);
    }

    public async Task DeleteMessageFromChannel(string channelCode, int deletedMessageId)
    {
        await Clients.Group(channelCode).SendAsync(_deleteMessageMethod, deletedMessageId);
    }

    public async Task JoinChannel(string channelCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, channelCode);

        var channel = await _sender.Send(new GetChannelByCodeQuery(channelCode));
        await Clients.Client(Context.ConnectionId).SendAsync(_joinChannelMethod, channel.Value);
    }

    public async Task CreateChannel(string channelCode)
    {
        if (Context.UserIdentifier == null)
            throw new UnauthorizedAccessException();

        GetChannelByCodeQuery channelQuery = new(channelCode);
        var channelResult = await _sender.Send(channelQuery);

        if (channelResult.Failed)
        {
            await Clients.User(Context.UserIdentifier).SendAsync(_onFailure);
            return;
        }

        var channel = channelResult.Value;

        GetAllMembershipsForChannelQuery membersQuery = new(channel.Id);
        var membersResult = await _sender.Send(membersQuery);
        var members = membersResult.Value;

        GetLatestChannelForMessageQuery messageQuery = new(channel.Id);
        var messageResult = await _sender.Send(messageQuery);
        var latestMessage = messageResult.Value;

        await Clients.Users(members.Select(e => e.Member.Id.ToString()).ToList())
                     .SendAsync(
                        _joinChannelMethod,
                        new Channel_BriefOverview(channel, latestMessage));
    }

    public async Task LeaveChannel(string channelCode)
    {
        GetChannelByCodeQuery channelQuery = new(channelCode);
        var channelResult = await _sender.Send(channelQuery);

        if (channelResult.Failed)
            return;

        IsUserMemberOfChannelQuery membershipQuery = new(int.Parse(Context.UserIdentifier!), channelResult.Value.Id);
        var isUserMember = await _sender.Send(membershipQuery);

        if (!isUserMember)
            return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelCode);
        await Clients.Client(Context.ConnectionId).SendAsync(_leaveChannelMethod, channelCode);
    }

    public async Task AddMemberToChannel(ChannelMember newMember)
    {
        string channelCode = newMember.Channel.Code;

        // Alert existing members which users were added
        await Clients.Group(channelCode)
                     .SendAsync(_addedToChannelMethod, channelCode, newMember);

        // Alert newly added users
        await Clients.Users(newMember.Member.Id.ToString())
                     .SendAsync(_addedToChannelMethod, channelCode, newMember);
    }

    public async Task RemoveMemberFromChannel(string channelCode, int removedMemberId)
    {
        await Clients.Group(channelCode)
                     .SendAsync(_removedFromChannelMethod, channelCode, removedMemberId);
    }
}