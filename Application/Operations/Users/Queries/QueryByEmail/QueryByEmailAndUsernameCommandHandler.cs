using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Queries.QueryByEmail;

internal class QueryByEmailAndUsernameCommandHandler : BaseCommandHandler, IRequestHandler<QueryByEmailAndUsernameCommand, IList<User>>
{
    public QueryByEmailAndUsernameCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<IList<User>> Handle(QueryByEmailAndUsernameCommand request, CancellationToken cancellationToken)
    {
        var queryResult = await _workUnit.UsersRepository.QueryByEmailAndUsernameAsync(request.Query);
        IList<User> users = new List<User>();

        foreach (var user in queryResult)
            users.Add(new(user.Id, user.UserName, user.Email));

        return users;
    }
}
