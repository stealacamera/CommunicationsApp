using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.ChannelMembers.Queries.GetAllMembershipsForUser;

public sealed class GetAllMembershipsForUserCommandHandler
    : BaseCommandHandler, IRequestHandler<GetAllMembershipsForUserCommand, Result<IList<ChannelMember>>>
{
    public GetAllMembershipsForUserCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<IList<ChannelMember>>> Handle(GetAllMembershipsForUserCommand request, CancellationToken cancellationToken)
    {
        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.UserId))
            return UserErrors.NotFound;

        var memberships = await _workUnit.ChannelMembersRepository.GetAllForUserAsync(request.UserId);

        return memberships.Select(async member =>
                            {
                                var user = await _workUnit.UsersRepository.GetByIdAsync(member.MemberId);
                                var channel = await _workUnit.ChannelsRepository.GetByIdAsync(member.ChannelId);

                                return new ChannelMember(
                                    new User(user.Id, user.UserName, user.Email),
                                    ChannelRole.FromValue(member.RoleId),
                                    new Channel_BriefDescription(channel.Id, channel.Name, channel.Code));
                            })
                          .Select(e => e.Result)
                          .ToList();
    }
}
