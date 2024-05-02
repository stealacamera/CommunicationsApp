using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Queries.GetUserById;

public sealed class GetUserByIdCommandHandler : BaseCommandHandler, IRequestHandler<GetUserByIdCommand, Result<User>>
{
    public GetUserByIdCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<User>> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(request.Id);

        return user == null
               ? UserErrors.NotFound
               : new User(user.Id, user.UserName, user.Email);
    }
}
