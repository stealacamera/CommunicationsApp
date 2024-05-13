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
                            _deleteMessageMethod = "DeleteMessage",
                            _joinChannelMethod = "JoinChannel";

    private readonly ISender _sender;

    public ChatHub(ISender sender)
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

    public async Task DeleteMessageFromChannel(int messageId, string channelCode)
    {
        await Clients.Group(channelCode).SendAsync(_deleteMessageMethod, messageId, channelCode);
    }

    public async Task JoinChannel(string channelcode)
    {
        // Check if user is member of channel
        if (Context.UserIdentifier == null)
            throw new UnauthorizedAccessException();

        GetChannelByCodeQuery channelQuery = new(channelcode);
        var channelResult = await _sender.Send(channelQuery);

        if (channelResult.Failed)
            return;

        IsUserMemberOfChannelQuery membershipQuery = new(int.Parse(Context.UserIdentifier), channelResult.Value.Id);
        var isUserMember = await _sender.Send(membershipQuery);

        if (!isUserMember)
            return;

        GetLatestChannelForMessageQuery messageQuery = new(channelResult.Value.Id);
        var messageResult = await _sender.Send(messageQuery);

        await Groups.AddToGroupAsync(Context.ConnectionId, channelcode);

        await Clients.Group(channelcode)
                     .SendAsync(
                        _joinChannelMethod,
                        new Channel_BriefOverview(channelResult.Value, messageResult.Value));
    }
}
