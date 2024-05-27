using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Queries.GetExternalAuthSchemes;

public record GetExternalAuthSchemesQuery() : IRequest<IEnumerable<AuthenticationScheme>>;
