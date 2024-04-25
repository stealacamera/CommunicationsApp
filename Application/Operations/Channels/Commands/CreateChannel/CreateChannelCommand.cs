using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Commands.CreateChannel;

public record CreateChannelCommand(int OwnerId, string Name, IList<int> MemberIds) : IRequest<Result<Channel>>;

public sealed class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
{
    public CreateChannelCommandValidator()
    {
        RuleFor(e => e.OwnerId).NotEmpty();

        RuleFor(e => e.Name)
            .MaximumLength(55)
            .NotEmpty();

        RuleFor(e => e.MemberIds).NotEmpty();
    }
}