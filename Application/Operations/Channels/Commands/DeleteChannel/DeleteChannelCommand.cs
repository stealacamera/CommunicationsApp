﻿using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Commands.DeleteChannel;

public record DeleteChannelCommand(int ChannelId) : IRequest<Result>;

public sealed class DeleteChannelCommandValidator : AbstractValidator<DeleteChannelCommand>
{
    public DeleteChannelCommandValidator()
    {
        RuleFor(e => e.ChannelId).NotEmpty();
    }
}