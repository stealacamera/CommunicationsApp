using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Entities;
using MediatR;
using System.Security.Claims;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ExternalSignUpUser;

internal sealed class ExternalSignUpCommandHandler : BaseCommandHandler, IRequestHandler<ExternalSignUpCommand, Result>
{
    public ExternalSignUpCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(ExternalSignUpCommand request, CancellationToken cancellationToken)
    {
        var loginInfo = await _workUnit.IdentityRepository.GetExternalLoginInfoAsync();

        if (loginInfo == null)
            return IdentityErrors.ExternalLoginError("");

        var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
            return IdentityErrors.ExternalLoginError(nameof(email));

        var user = new User
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await _workUnit.UsersRepository.AddAsync(user);

        if (!createResult.Succeeded)
            return createResult;

        await _workUnit.SaveChangesAsync();
        await _workUnit.IdentityRepository.SignInUserAsync(user);

        return Result.Success();
    }
}