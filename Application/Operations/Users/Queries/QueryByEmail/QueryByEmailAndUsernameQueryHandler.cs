using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Queries.QueryByEmail;

internal class QueryByEmailAndUsernameQueryHandler : BaseCommandHandler, IRequestHandler<QueryByEmailAndUsernameQuery, IList<User>>
{
    public QueryByEmailAndUsernameQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<IList<User>> Handle(QueryByEmailAndUsernameQuery request, CancellationToken cancellationToken)
    {
        var queryResult = await _workUnit.UsersRepository
                                         .QueryByEmailAndUsernameAsync(request.Query, request.excludeRequesterId);

        return queryResult.Select(e => new User(e.Id, e.UserName, e.Email))
                          .ToList();
    }
}
