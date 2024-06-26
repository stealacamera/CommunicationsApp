﻿using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Commands.CreateChannel;

public record CreateChannelCommand(int OwnerId, string Name, IList<int> MemberIds) : IRequest<Result<Channel_BriefDescription>>;

public sealed class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
{
    public CreateChannelCommandValidator(int maxNrMembers) : base()
    {
        RuleFor(e => e.OwnerId)
            .NotEmpty();

        RuleFor(e => e.Name)
            .MaximumLength(55)
            .NotEmpty();

        RuleFor(e => e.MemberIds)
            .NotEmpty()
            .Must(e => e.Any() && e.Count <= maxNrMembers);
    }
}