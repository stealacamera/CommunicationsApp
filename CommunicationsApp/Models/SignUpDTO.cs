using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel;

namespace CommunicationsApp.Web.Models;

public record SignUpDTO
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    [DisplayName("Confirm password")]
    public string ConfirmPassword { get; set; } = null!;

    public IEnumerable<AuthenticationScheme> AuthSchemes { get; set; } = null!;
}

internal class SignUpValidator : AbstractValidator<SignUpDTO>
{
    public SignUpValidator()
    {
        RuleFor(e => e.Username)
            .NotEmpty()
            .MaximumLength(45);

        RuleFor(e => e.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(e => e.Password)
            .NotEmpty()
            .MaximumLength(70);

        RuleFor(e => e.ConfirmPassword)
            .NotEmpty()
            .Equal(e => e.Password)
            .WithMessage("Passwords should be the same");
    }
}