using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ConfigureExternalAuthProperties;

public record ConfigureExternalAuthPropertiesCommand(
    string Provider,
    string RedirectUrl)
    : IRequest<AuthenticationProperties>;

internal class ConfigureExternalAuthPropertiesCommandValidator : AbstractValidator<ConfigureExternalAuthPropertiesCommand>
{
    public ConfigureExternalAuthPropertiesCommandValidator()
    {
        RuleFor(e => e.Provider)
            .NotEmpty();

        RuleFor(e => e.RedirectUrl)
            .NotEmpty();
    }
}