using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Username,
    string Password,
    string Email,
    string EmailConfirmationBaseUrl)
    : IRequest<Result<User>>;

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(e => e.Username)
            .MaximumLength(45)
            .NotEmpty();

        RuleFor(e => e.Password)
            .MaximumLength(70)
            .NotEmpty();

        RuleFor(e => e.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(e => e.EmailConfirmationBaseUrl)
            .NotEmpty();
    }
}
