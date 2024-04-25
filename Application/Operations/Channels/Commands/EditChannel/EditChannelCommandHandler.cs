using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Commands.EditChannel;

public sealed class EditChannelCommandHandler : BaseCommandHandler, IRequestHandler<EditChannelCommand, Result>
{
    public EditChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(EditChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId);

        if (channel == null)
            return ChannelErrors.NotFound;

        channel.Name = request.NewName;
        await _workUnit.SaveChangesAsync();

        return Result.Success();
    }
}
