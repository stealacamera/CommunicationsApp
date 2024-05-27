using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Queries.GetChannelByCode;

internal sealed class GetChannelByCodeQueryHandler
    : BaseCommandHandler, IRequestHandler<GetChannelByCodeQuery, Result<Channel_BriefDescription>>
{
    public GetChannelByCodeQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Channel_BriefDescription>> Handle(GetChannelByCodeQuery request, CancellationToken cancellationToken)
    {
        GetChannelByCodeQueryValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return validationResult;

        var channel = await _workUnit.ChannelsRepository
                                     .GetByCodeAsync(request.ChannelCode, cancellationToken);

        return channel == null
               ? ChannelErrors.NotFound(nameof(request.ChannelCode))
               : new Channel_BriefDescription(channel.Id, channel.Name, channel.Code);
    }
}
