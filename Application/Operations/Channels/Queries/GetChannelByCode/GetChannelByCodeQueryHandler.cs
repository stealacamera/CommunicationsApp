using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Queries.GetChannelByCode;

internal sealed class GetChannelByCodeQueryHandler 
    : BaseCommandHandler, IRequestHandler<GetChannelByCodeQuery, Result<Channel_BriefDescription>>
{
    public GetChannelByCodeQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Channel_BriefDescription>> Handle(GetChannelByCodeQuery request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository.GetByCodeAsync(request.ChannelCode);

        return channel == null
               ? ChannelErrors.NotFound
               : new Channel_BriefDescription(channel.Id, channel.Name, channel.Code);
    }
}
