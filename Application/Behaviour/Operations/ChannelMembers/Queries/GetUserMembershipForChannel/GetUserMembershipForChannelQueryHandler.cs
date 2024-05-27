using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.GetUserMembershipForChannel;

internal class GetUserMembershipForChannelQueryHandler : BaseCommandHandler, IRequestHandler<GetUserMembershipForChannelQuery, Result<ChannelMember?>>
{
    public GetUserMembershipForChannelQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<ChannelMember?>> Handle(GetUserMembershipForChannelQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await new GetUserMembershipForChannelQueryValidator().ValidateAsync(request);

        if (!validationResult.IsValid)
            return validationResult;

        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.UserId))
            return UserErrors.NotFound(nameof(request.UserId));

        if (!await _workUnit.ChannelsRepository.DoesInstanceExistAsync(request.ChannelId, cancellationToken))
            return ChannelErrors.NotFound(nameof(request.ChannelId));

        var membership = await _workUnit.ChannelMembersRepository
                                        .GetByIdsAsync(request.UserId, request.ChannelId, cancellationToken);

        if(membership != null)
        {
            var user = (await _workUnit.UsersRepository.GetByIdAsync(request.UserId))!;
            var channel = (await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId, cancellationToken))!;

            return new ChannelMember(
                new User(user.Id, user.UserName, user.Email), 
                ChannelRole.FromValue(membership.RoleId), 
                new Channel_BriefDescription(channel.Id, channel.Name, channel.Code));
        }

        return Result<ChannelMember?>.Success(null);
    }
}
