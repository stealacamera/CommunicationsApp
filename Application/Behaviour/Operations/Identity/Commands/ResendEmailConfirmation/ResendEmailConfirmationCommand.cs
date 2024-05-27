using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ResendEmailConfirmation;

public sealed record ResendEmailConfirmationCommand(string Email, string EmailConfirmationUrl) : IRequest<bool>;

internal sealed class ResendEmailConfirmationCommandValidator : AbstractValidator<ResendEmailConfirmationCommand>
{
    public ResendEmailConfirmationCommandValidator()
    {
        RuleFor(e => e.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(e => e.EmailConfirmationUrl)
            .NotEmpty();
    }
}