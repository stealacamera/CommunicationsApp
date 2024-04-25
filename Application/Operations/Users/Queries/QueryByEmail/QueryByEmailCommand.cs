using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Queries.QueryByEmail;

public record QueryByEmailCommand : IRequest<IList<UserDTO>>
{
    public string Query;
}