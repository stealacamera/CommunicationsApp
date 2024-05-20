using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Queries.GetChannelById;

public class GetChannelByIdQueryHandler : BaseCommandHandler, IRequestHandler<GetChannelByIdQuery, Result<Channel>>
{
    public GetChannelByIdQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Channel>> Handle(GetChannelByIdQuery request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken);

        if (channel == null)
            return ChannelErrors.NotFound;

        if (!await _workUnit.ChannelMembersRepository
                            .IsUserMemberOfChannelAsync(request.RequesterId, request.ChannelId, cancellationToken))
            return ChannelMemberErrors.NotMemberOfChannel;

        // Get members        
        var channelMembers = await _workUnit.ChannelMembersRepository
                                            .GetAllForChannelAsync(channel.Id, cancellationToken);

        var members = channelMembers.Select(async member =>
                                        {
                                            var user = await _workUnit.UsersRepository.GetByIdAsync(member.MemberId);

                                            return new ChannelMember(
                                                new User(user.Id, user.UserName, user.Email),
                                                ChannelRole.FromValue(member.RoleId),
                                                new Channel_BriefDescription(channel.Id, channel.Name, channel.Code));
                                        })
                                    .Select(e => e.Result)
                                    .ToList();

        return new Channel(channel.Id, channel.Name, channel.CreatedAt, channel.Code, members);
    }
}
