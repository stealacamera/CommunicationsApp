using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Queries.GetChannelById;

public class GetChannelByIdCommandHandler : BaseCommandHandler, IRequestHandler<GetChannelByIdCommand, Result<Channel>>
{
    public GetChannelByIdCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Channel>> Handle(GetChannelByIdCommand request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId, includeMembers: true);

        if (channel == null)
            return ChannelErrors.NotFound;

        if (!await _workUnit.ChannelsRepository.DoesUserBelongToChannelAsync(request.RequesterId, request.ChannelId))
            return ChannelMemberErrors.UserIsNotMemberOfChannel;

        var channelOwner = await _workUnit.UsersRepository.GetByIdAsync(channel.OwnerId);
        User owner = new(channelOwner.Id, channelOwner.UserName, channelOwner.Email);

        IList<User> members = new List<User>{ owner };

        foreach (var member in channel.Members)
            members.Add(new User(member.Id, member.UserName, member.Email));

        return new Channel(channel.Id, channel.Name, channel.CreatedAt, channel.Code, owner, members);
    }
}
