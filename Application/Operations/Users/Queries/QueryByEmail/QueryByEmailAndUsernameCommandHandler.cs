using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Queries.QueryByEmail;

internal class QueryByEmailAndUsernameCommandHandler : BaseCommandHandler, IRequestHandler<QueryByEmailAndUsernameCommand, IList<UserDTO>>
{
    public QueryByEmailAndUsernameCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<IList<UserDTO>> Handle(QueryByEmailAndUsernameCommand request, CancellationToken cancellationToken)
    {
        var queryResult = await _workUnit.UsersRepository.QueryByEmailAndUsernameAsync(request.Query);
        IList<UserDTO> users = new List<UserDTO>();

        foreach (var user in queryResult)
            users.Add(new()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            });

        return users;
    }
}
