using CommunicationsApp.Application.Common.Exceptions;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Entities;
using CommunicationsApp.Domain.Events;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Users.Commands;

public record CreateUserCommand(
    string Username, 
    string Password, 
    string Email, 
    string EmailConfirmationBaseUrl) 
    : IRequest<User>;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
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
            .EmailAddress();

        RuleFor(e => e.EmailConfirmationBaseUrl)
            .NotEmpty();
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IWorkUnit _workUnit;
    private readonly IPublisher _publisher;

    public CreateUserCommandHandler(IWorkUnit workUnit, IPublisher publisher)
    {
        _workUnit = workUnit;
        _publisher = publisher;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
        };

        var createUserResult = await _workUnit.UsersRepository.AddAsync(user, request.Password);

        if (createUserResult.Succeeded)
        {
            await _workUnit.SaveChangesAsync();
            await _publisher.Publish(new UserAdded(user.Id, user.Email, request.EmailConfirmationBaseUrl, DateTime.Now));

            return user;
        }
        else
            throw new UserValidationException(createUserResult.Errors);
    }
}
