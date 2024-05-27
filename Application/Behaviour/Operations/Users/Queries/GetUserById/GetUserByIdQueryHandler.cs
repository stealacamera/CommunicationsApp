using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Users.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler : BaseCommandHandler, IRequestHandler<GetUserByIdQuery, Result<User>>
{
    public GetUserByIdQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        GetUserByIdQueryValidator validator = new();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult;

        var user = await _workUnit.UsersRepository
                                  .GetByIdAsync(request.Id);

        return user == null ? 
               UserErrors.NotFound(nameof(request.Id)) : 
               new User(user.Id, user.UserName, user.Email);
    }
}
