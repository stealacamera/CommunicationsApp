using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Commands.EditChannel;

public record EditChannelCommand(int RequesterId, int ChannelId, string NewName) : IRequest<Result>;

internal sealed class EditChannelCommandValidator : AbstractValidator<EditChannelCommand>
{
    public EditChannelCommandValidator()
    {
        RuleFor(e => e.RequesterId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.NewName)
            .NotEmpty()
            .MaximumLength(55);
    }
}