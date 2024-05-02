using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Queries.GetAllChannelsForUser;

public sealed class GetAllChannelsForUserCommadHandler
    : BaseCommandHandler, IRequestHandler<GetAllChannelsForUserCommad, Result<IList<Channel_BriefDescription>>>
{
    public GetAllChannelsForUserCommadHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<IList<Channel_BriefDescription>>> Handle(GetAllChannelsForUserCommad request, CancellationToken cancellationToken)
    {
        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.UserId))
            return UserErrors.NotFound;

        var userChannels = (await _workUnit.ChannelsRepository
                                           .GetAllForUser(request.UserId))
                                           .Select(e => new Channel_BriefDescription(e.Id, e.Name))
                                           .ToList();

        return userChannels;
    }
}
