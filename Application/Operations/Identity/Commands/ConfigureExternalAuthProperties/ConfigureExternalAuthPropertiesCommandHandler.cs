using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace CommunicationsApp.Application.Operations.Identity.Commands.ConfigureExternalAuthProperties;

internal sealed class ConfigureExternalAuthPropertiesCommandHandler 
    : BaseCommandHandler, IRequestHandler<ConfigureExternalAuthPropertiesCommand, AuthenticationProperties>
{
    public ConfigureExternalAuthPropertiesCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public Task<AuthenticationProperties> Handle(ConfigureExternalAuthPropertiesCommand request, CancellationToken cancellationToken)
        => Task.Run(() => _workUnit.UsersRepository.ConfigureExternalAuthProperties(request.Provider, request.RedirectUrl));
}
