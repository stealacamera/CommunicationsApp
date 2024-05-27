using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommmand(int UserId, string EmailConfirmationToken) : IRequest<bool>;

internal sealed class ConfirmEmailCommmandValidator : AbstractValidator<ConfirmEmailCommmand>
{
    public ConfirmEmailCommmandValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.EmailConfirmationToken).NotEmpty();
    }
}