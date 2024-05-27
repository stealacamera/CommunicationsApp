using FluentValidation;
using Microsoft.AspNetCore.Authentication;

namespace CommunicationsApp.Web.Models;

public record LoginDTO
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; } = false;

    public IEnumerable<AuthenticationScheme> AuthSchemes { get; set; } = null!;
}

internal class LoginValidator : AbstractValidator<LoginDTO>
{
    public LoginValidator()
    {
        RuleFor(e => e.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(e => e.Password)
            .NotEmpty()
            .MaximumLength(75);
    }
}