using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ConfigureExternalAuthProperties;

internal sealed class ConfigureExternalAuthPropertiesCommandHandler
    : BaseCommandHandler, IRequestHandler<ConfigureExternalAuthPropertiesCommand, AuthenticationProperties>
{
    public ConfigureExternalAuthPropertiesCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<AuthenticationProperties> Handle(ConfigureExternalAuthPropertiesCommand request, CancellationToken cancellationToken)
    {
        ConfigureExternalAuthPropertiesCommandValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ArgumentException("Invalid authentication request");

        return _workUnit.IdentityRepository.ConfigureExternalAuthProperties(request.Provider, request.RedirectUrl);
    }
}
