﻿using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Commands.DeleteChannel;

public sealed class DeleteChannelCommandHandler : BaseCommandHandler, IRequestHandler<DeleteChannelCommand, Result>
{
    public DeleteChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken);

        if (channel == null)
            return ChannelErrors.NotFound;

        var membership = await _workUnit.ChannelMembersRepository
                                        .GetByIdsAsync(request.RequesterId, channel.Id, cancellationToken);

        if (membership == null || membership.RoleId != ChannelRole.Owner)
            return UserErrors.Unauthorized;

        channel.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return Result.Success();
    }
}