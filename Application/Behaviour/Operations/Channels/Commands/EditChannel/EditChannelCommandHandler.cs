using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Commands.EditChannel;

internal sealed class EditChannelCommandHandler : BaseCommandHandler, IRequestHandler<EditChannelCommand, Result>
{
    public EditChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(EditChannelCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await IsRequestValidAsync(request, cancellationToken);

        if (validationResult.Failed)
            return validationResult;

        var channel = (await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken))!;

        channel.Name = request.NewName;
        await _workUnit.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<Result> IsRequestValidAsync(EditChannelCommand request, CancellationToken cancellationToken)
    {
        EditChannelCommandValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return validationResult;

        var channel = await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken);

        if (channel == null)
            return ChannelErrors.NotFound(nameof(request.ChannelId));

        var membership = await _workUnit.ChannelMembersRepository
                                        .GetByIdsAsync(request.RequesterId, channel.Id, cancellationToken);

        if (membership == null || membership.RoleId != ChannelRole.Owner)
            return UserErrors.Unauthorized(nameof(request.RequesterId));

        return Result.Success();
    }
}
