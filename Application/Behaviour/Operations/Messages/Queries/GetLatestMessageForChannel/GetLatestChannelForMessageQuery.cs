using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Messages.Queries.GetLatestMessageForChannel;

public record GetLatestChannelForMessageQuery(int ChannelId) : IRequest<Result<Message?>>;

internal sealed class GetLatestChannelForMessageQueryValidation : AbstractValidator<GetLatestChannelForMessageQuery>
{
    public GetLatestChannelForMessageQueryValidation()
    {
        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);
    }
}