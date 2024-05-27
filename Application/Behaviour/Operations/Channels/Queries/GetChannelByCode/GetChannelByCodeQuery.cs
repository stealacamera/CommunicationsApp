using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Queries.GetChannelByCode;

public record GetChannelByCodeQuery(string ChannelCode) : IRequest<Result<Channel_BriefDescription>>;

internal sealed class GetChannelByCodeQueryValidator : AbstractValidator<GetChannelByCodeQuery>
{
    public GetChannelByCodeQueryValidator()
    {
        RuleFor(e => e.ChannelCode)
            .NotEmpty()
            .MaximumLength(50);
    }
}