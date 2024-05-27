using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.GetAllMembershipsForChannel;

internal class GetAllMembershipsForChannelQueryHandler : BaseCommandHandler, IRequestHandler<GetAllMembershipsForChannelQuery, Result<IList<ChannelMember>>>
{
    public GetAllMembershipsForChannelQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<IList<ChannelMember>>> Handle(GetAllMembershipsForChannelQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await IsRequestValidAsync(request, cancellationToken);

        if (validationResult.Failed)
            return validationResult.Errors.ToArray();

        var channel = (await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId))!;
        var channelModel = new Channel_BriefDescription(channel.Id, channel.Name, channel.Code);

        var members = await _workUnit.ChannelMembersRepository
                                     .GetAllForChannelAsync(request.ChannelId, cancellationToken);

        return members.Select(async e => await ConvertToModel(e, channelModel))
                      .Select(e => e.Result)
                      .ToList();
    }

    private async Task<ChannelMember> ConvertToModel(Domain.Entities.ChannelMember entity, Channel_BriefDescription channelModel)
    {
        var user = (await _workUnit.UsersRepository.GetByIdAsync(entity.MemberId))!;
        var userModel = new User(user.Id, user.UserName, user.Email);

        return new ChannelMember(userModel, ChannelRole.FromValue(entity.RoleId), channelModel);
    }

    private async Task<Result> IsRequestValidAsync(GetAllMembershipsForChannelQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await new GetAllMembershipsForChannelQueryValidator()
                                            .ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult;

        if (!await _workUnit.ChannelsRepository
                            .DoesInstanceExistAsync(request.ChannelId, cancellationToken))
            return ChannelErrors.NotFound(nameof(request.ChannelId));

        return Result.Success();
    }
}
