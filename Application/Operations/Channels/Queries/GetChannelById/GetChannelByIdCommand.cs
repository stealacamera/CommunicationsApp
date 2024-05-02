using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Queries.GetChannelById;

public record GetChannelByIdCommand(int ChannelId, int RequesterId) : IRequest<Result<Channel>>;

public sealed class GetChannelByIdCommandValidator : AbstractValidator<GetChannelByIdCommand>
{
    public GetChannelByIdCommandValidator()
    {
        RuleFor(e => e.ChannelId).NotEmpty();
        RuleFor(e => e.RequesterId).NotEmpty();
    }
}
