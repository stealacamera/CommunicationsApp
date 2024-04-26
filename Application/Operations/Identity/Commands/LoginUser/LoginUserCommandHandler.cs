using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Identity.Commands.LoginUser;

public sealed class LoginUserCommandHandler : BaseCommandHandler, IRequestHandler<LoginUserCommand, Result<bool>>
{
    public LoginUserCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<bool>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByEmailAsync(request.Email, excludeNonConfirmedEmail: false);

        if (user == null)
            return UserErrors.NotFound;
        else if (user.EmailConfirmed == false)
            return IdentityErrors.UnverifiedEmail;

        var signInResult = await _workUnit.UsersRepository.SignInUserAsync(user, request.Password, request.rememberMe);
        return signInResult.Succeeded ? true : false;
    }
}
