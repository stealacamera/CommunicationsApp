using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : BaseCommandHandler, IRequestHandler<GetUserByIdQuery, Result<User>>
{
    public GetUserByIdQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(request.Id);

        return user == null
               ? UserErrors.NotFound
               : new User(user.Id, user.UserName, user.Email);
    }
}
