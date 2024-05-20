using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Operations.ChannelMembers.Commands.RemoveMembers;

internal class RemoveChannelMembersCommandHandler : BaseCommandHandler, IRequestHandler<RemoveChannelMembersCommand, Result<IList<int>>>
{
    private readonly IPublisher _publisher;

    public RemoveChannelMembersCommandHandler(
        IWorkUnit workUnit,
        IPublisher publisher) 
        : base(workUnit)
    {
        _publisher = publisher;
    }

    public async Task<Result<IList<int>>> Handle(RemoveChannelMembersCommand request, CancellationToken cancellationToken)
    {
        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.RequesterId))
            return UserErrors.Unauthorized;
        else if (!await _workUnit.ChannelsRepository.DoesInstanceExistAsync(request.ChannelId, cancellationToken))
            return ChannelErrors.NotFound;

        var nrMembers = await _workUnit.ChannelMembersRepository.GetCountForChannelAsync(request.ChannelId, cancellationToken);

        if (request.MemberIds.Length >= (nrMembers - 1))
            return ChannelMemberErrors.CannotRemoveAllMembers;

        var requesterMembership = await _workUnit.ChannelMembersRepository
                                                 .GetByIdsAsync(request.RequesterId, request.ChannelId, cancellationToken);

        if(requesterMembership == null || requesterMembership.RoleId != ChannelRole.Owner)
            return UserErrors.Unauthorized;

        var removedMemberIds = await RemoveMemberships(request.MemberIds, request.ChannelId, cancellationToken);
        return (Result<IList<int>>)removedMemberIds;
    }

    private async Task<IList<int>> RemoveMemberships(int[] memberIds, int channelId, CancellationToken cancellationToken)
    {
        IList<int> removedMemberIds = new List<int>();

        foreach (var memberId in memberIds)
        {
            var membership = await _workUnit.ChannelMembersRepository
                                            .GetByIdsAsync(memberId, channelId, cancellationToken);

            if (membership != null && membership.RoleId != ChannelRole.Owner)
            {
                _workUnit.ChannelMembersRepository.Remove(membership);
                await _workUnit.SaveChangesAsync();

                removedMemberIds.Add(memberId);

                var user = await _workUnit.UsersRepository
                                          .GetByIdAsync(membership.MemberId);

                await _publisher.Publish(
                    new MemberRemovedFromChannel(user.Email, membership.ChannelId, DateTime.Now),
                    cancellationToken);
            }
        }

        return removedMemberIds;
    }
}
