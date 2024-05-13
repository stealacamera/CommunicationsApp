using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace CommunicationsApp.Application.Operations.Identity.Commands.ConfigureExternalAuthProperties;

public record ConfigureExternalAuthPropertiesCommand(
    string Provider, 
    string RedirectUrl) 
    : IRequest<AuthenticationProperties>;