using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Commands.RemoveMembers;

internal class RemoveChannelMembersCommandHandler : BaseCommandHandler, IRequestHandler<RemoveChannelMembersCommand, Result>
{
    private readonly IPublisher _publisher;

    public RemoveChannelMembersCommandHandler(
        IWorkUnit workUnit,
        IPublisher publisher)
        : base(workUnit)
    {
        _publisher = publisher;
    }

    public async Task<Result> Handle(RemoveChannelMembersCommand request, CancellationToken cancellationToken)
    {
        var requestValidation = await IsRequestValidAsync(request, cancellationToken);

        if (requestValidation.Failed)
            return requestValidation.Errors.ToArray();
        
        await RemoveMemberships(request.MemberIds, request.ChannelId, cancellationToken);
        return Result.Success();
    }

    private async Task<Result> IsRequestValidAsync(RemoveChannelMembersCommand request, CancellationToken cancellationToken)
    {
        RemoveChannelMembersCommandValidator requestValidator = new();
        var validationResult = await requestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return validationResult;

        if (!await _workUnit.ChannelsRepository.DoesInstanceExistAsync(request.ChannelId, cancellationToken))
            return ChannelErrors.NotFound(nameof(request.ChannelId));
        
        if (!await IsRequesterAuthorized(request.RequesterId, request.ChannelId, cancellationToken))
            return UserErrors.Unauthorized(nameof(request.RequesterId));

        var nrTotalMembers = await _workUnit.ChannelMembersRepository
                                       .GetCountForChannelAsync(request.ChannelId, cancellationToken);

        if (request.MemberIds.Length >= nrTotalMembers - 1)
            return ChannelMemberErrors.CannotRemoveAllMembers(nameof(request.MemberIds));

        return Result.Success();
    }

    private async Task<bool> IsRequesterAuthorized(int requesterId, int channelId, CancellationToken cancellationToken)
    {
        if (!await _workUnit.UsersRepository.DoesUserExistAsync(requesterId))
            return false;

        var requesterMembership = await _workUnit.ChannelMembersRepository
                                                 .GetByIdsAsync(requesterId, channelId, cancellationToken);

        if (requesterMembership == null || requesterMembership.RoleId != ChannelRole.Owner)
            return false;

        return true;
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
