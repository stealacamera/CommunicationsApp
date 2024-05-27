using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.GetAllMembershipsForUser;

internal sealed class GetAllMembershipsForUserQuerydHandler
    : BaseCommandHandler, IRequestHandler<GetAllMembershipsForUserQuery, Result<IList<ChannelMember>>>
{
    public GetAllMembershipsForUserQuerydHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<IList<ChannelMember>>> Handle(GetAllMembershipsForUserQuery request, CancellationToken cancellationToken)
    {
        GetAllMembershipsForUserQueryValidator requestValidator = new();
        var requestValidation = await requestValidator.ValidateAsync(request);

        if (!requestValidation.IsValid)
            return requestValidation;

        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.UserId))
            return UserErrors.NotFound(nameof(request.UserId));

        var memberships = await _workUnit.ChannelMembersRepository
                                         .GetAllForUserAsync(request.UserId, cancellationToken);

        return memberships.Select(async member => await ConvertEntityToModel(member, cancellationToken))
                          .Select(e => e.Result)
                          .ToList();
    }

    private async Task<ChannelMember> ConvertEntityToModel(Domain.Entities.ChannelMember member, CancellationToken cancellationToken)
    {
        var user = (await _workUnit.UsersRepository.GetByIdAsync(member.MemberId))!;
        var channel = (await _workUnit.ChannelsRepository.GetByIdAsync(member.ChannelId, cancellationToken))!;
        
        return new ChannelMember(
            new User(user.Id, user.UserName, user.Email),
            ChannelRole.FromValue(member.RoleId),
            new Channel_BriefDescription(channel.Id, channel.Name, channel.Code));
    }
}
