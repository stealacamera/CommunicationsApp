﻿using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Queries.GetChannelById;

public record GetChannelByIdQuery(int ChannelId, int RequesterId) : IRequest<Result<Channel>>;

internal sealed class GetChannelByIdQueryValidator : AbstractValidator<GetChannelByIdQuery>
{
    public GetChannelByIdQueryValidator()
    {
        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.RequesterId)
            .NotEmpty()
            .GreaterThan(0);
    }
}
