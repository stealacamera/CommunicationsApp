using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;
using CommunicationsApp.Application.Common.Results.Errors;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.LoginUser;

internal sealed class LoginUserCommandHandler : BaseCommandHandler, IRequestHandler<LoginUserCommand, Result<bool>>
{
    public LoginUserCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<bool>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        LoginUserCommandValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return validationResult;

        var user = await _workUnit.UsersRepository
                                  .GetByEmailAsync(request.Email, excludeNonConfirmedEmail: false);

        if (user == null)
            return UserErrors.NotFound(nameof(request.Email));
        else if (user.EmailConfirmed == false)
            return IdentityErrors.UnverifiedEmail(nameof(request.Email));

        var signInResult = await _workUnit.IdentityRepository
                                          .SignInUserAsync(user, request.Password, request.rememberMe);
        
        return signInResult.Succeeded;
    }
}
