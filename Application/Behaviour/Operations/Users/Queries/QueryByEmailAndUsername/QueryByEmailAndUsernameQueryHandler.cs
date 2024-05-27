using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Users.Queries.QueryByEmailAndUsername;

internal class QueryByEmailAndUsernameQueryHandler : BaseCommandHandler, IRequestHandler<QueryByEmailAndUsernameQuery, IList<User>>
{
    public QueryByEmailAndUsernameQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<IList<User>> Handle(QueryByEmailAndUsernameQuery request, CancellationToken cancellationToken)
    {
        QueryByEmailAndUsernameQueryValidator validator = new();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var queryResult = await _workUnit.UsersRepository
                                         .QueryByEmailAndUsernameAsync(
                                            request.Query,
                                            request.ExcludeRequesterId,
                                            cancellationToken: cancellationToken);

        return queryResult.Select(e => new User(e.Id, e.UserName, e.Email))
                          .ToList();
    }
}
