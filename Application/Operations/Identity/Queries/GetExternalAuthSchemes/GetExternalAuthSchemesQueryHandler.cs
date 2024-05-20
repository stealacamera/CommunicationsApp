using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace CommunicationsApp.Application.Operations.Identity.Queries.GetExternalAuthSchemes;

internal sealed class GetExternalAuthSchemesQueryHandler : BaseCommandHandler, IRequestHandler<GetExternalAuthSchemesQuery, IEnumerable<AuthenticationScheme>>
{
    public GetExternalAuthSchemesQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<IEnumerable<AuthenticationScheme>> Handle(GetExternalAuthSchemesQuery request, CancellationToken cancellationToken)
        => await _workUnit.IdentityRepository.GetExternalAuthSchemesAsync();
}
