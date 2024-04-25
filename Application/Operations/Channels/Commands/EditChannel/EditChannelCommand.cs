using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Commands.EditChannel;

public record EditChannelCommand(int ChannelId, string NewName) : IRequest<Result>;

public sealed class EditChannelCommandValidator : AbstractValidator<EditChannelCommand>
{
    public EditChannelCommandValidator()
    {
        RuleFor(e => e.ChannelId).NotEmpty();

        RuleFor(e => e.NewName)
            .NotEmpty()
            .MaximumLength(55);
    }
}