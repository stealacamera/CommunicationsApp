using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;

public sealed class GetAllMessagesForChannelCommandHandler : BaseCommandHandler, IRequestHandler<GetAllMessagesForChannelCommand, Result>
{
    public GetAllMessagesForChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(GetAllMessagesForChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId);

        //check if user is owner or member of channel

        if (channel == null)
            return ChannelErrors.NotFound;
        else if (channel.OwnerId != 0)
            ;

        return Result.Success();
    }
}