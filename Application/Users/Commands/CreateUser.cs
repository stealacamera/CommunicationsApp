using CommunicationsApp.Application.Common.Exceptions;
using CommunicationsApp.Domain.Entities;
using CommunicationsApp.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Application.Users.Commands;

public record CreateUserCommand : IRequest<User>
{
    [Required]
    [StringLength(45)]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}

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
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IWorkUnit _workUnit;
    private readonly IEmailSender _emailSender;

    public CreateUserCommandHandler(IWorkUnit workUnit, IEmailSender emailSender)
    {
        _workUnit = workUnit;
        _emailSender = emailSender;
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
            return user;
        }
        else
            throw new UserValidationException(createUserResult.Errors);
    }
}
