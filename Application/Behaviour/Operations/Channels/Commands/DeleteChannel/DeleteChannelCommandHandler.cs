using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Commands.DeleteChannel;

internal sealed class DeleteChannelCommandHandler : BaseCommandHandler, IRequestHandler<DeleteChannelCommand, Result>
{
    public DeleteChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        var requestValidation = await IsRequestValidAsync(request, cancellationToken);

        if (requestValidation.Failed)
            return requestValidation;

        var channel = (await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken))!;

        channel.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<Result> IsRequestValidAsync(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        // Validate properties
        DeleteChannelCommandValidator requestValidator = new();
        var validationResult = await requestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return validationResult;

        // Check if channel exists
        var channel = await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken);

        if (channel == null)
            return ChannelErrors.NotFound(nameof(request.ChannelId));

        // Check if requester is channel owner
        var membership = await _workUnit.ChannelMembersRepository
                                        .GetByIdsAsync(request.RequesterId, channel.Id, cancellationToken);

        if (membership == null || membership.RoleId != ChannelRole.Owner)
            return UserErrors.Unauthorized(nameof(request.RequesterId));

        return Result.Success();
    }
}