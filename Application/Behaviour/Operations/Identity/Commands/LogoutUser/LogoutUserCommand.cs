using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.LogoutUser;

public record LogoutUserCommand(int RequesterId) : IRequest;

internal class LogoutUserCommandValidator : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserCommandValidator()
    {
        RuleFor(e => e.RequesterId)
            .NotEmpty()
            .GreaterThan(0);
    }
}