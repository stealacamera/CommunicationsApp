using CommunicationsApp.Application.Common;
using CommunicationsApp.Infrastructure;
using MediatR;

namespace CommunicationsApp.Application.Users.Queries;

public record QueryByEmailCommand : IRequest<IList<UserDTO>>
{
    public string Query;
}

internal class QueryByEmailCommandHandler : BaseCommandHandler, IRequestHandler<QueryByEmailCommand, IList<UserDTO>>
{
    public QueryByEmailCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<IList<UserDTO>> Handle(QueryByEmailCommand request, CancellationToken cancellationToken)
    {
        var queryResult = await _workUnit.UsersRepository.QueryByEmail(request.Query);

        IList<UserDTO> users = new List<UserDTO>();

        foreach (var user in queryResult)
            users.Add(new()
            {
                Email = user.Email,
                Id = user.Id,
                UserName = user.UserName
            });

        return users;
    }
}
