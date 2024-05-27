using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Queries.GetChannelById;

internal class GetChannelByIdQueryHandler : BaseCommandHandler, IRequestHandler<GetChannelByIdQuery, Result<Channel>>
{
    public GetChannelByIdQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Channel>> Handle(GetChannelByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await IsRequestValidAsync(request, cancellationToken);

        if (validationResult.Failed)
            return validationResult.Errors.ToArray();

        var channel = (await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken))!;

        var channelMembers = await _workUnit.ChannelMembersRepository
                                            .GetAllForChannelAsync(channel.Id, cancellationToken);

        var members = channelMembers.Select(async member =>
                                        {
                                            var user = (await _workUnit.UsersRepository.GetByIdAsync(member.MemberId))!;

                                            return new ChannelMember(
                                                new User(user.Id, user.UserName, user.Email),
                                                ChannelRole.FromValue(member.RoleId),
                                                new Channel_BriefDescription(channel.Id, channel.Name, channel.Code));
                                        })
                                    .Select(e => e.Result)
                                    .ToList();

        return new Channel(channel.Id, channel.Name, channel.CreatedAt, channel.Code, members);
    }

    private async Task<Result> IsRequestValidAsync(GetChannelByIdQuery request, CancellationToken cancellationToken)
    {
        GetChannelByIdQueryValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return validationResult;

        var channel = await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken);

        if (channel == null)
            return ChannelErrors.NotFound(nameof(request.ChannelId));

        if (!await _workUnit.ChannelMembersRepository
                            .IsUserMemberOfChannelAsync(request.RequesterId, request.ChannelId, cancellationToken))
            return ChannelMemberErrors.NotMemberOfChannel(nameof(request.RequesterId));

        return Result.Success();
    }
}
