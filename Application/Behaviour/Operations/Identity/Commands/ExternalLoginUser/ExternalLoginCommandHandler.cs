﻿using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;
using System.Security.Claims;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ExternalLoginUser;

internal sealed class ExternalLoginCommandHandler : BaseCommandHandler, IRequestHandler<ExternalLoginCommand, Result>
{
    public ExternalLoginCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        var loginInfo = await _workUnit.IdentityRepository.GetExternalLoginInfoAsync();

        if (loginInfo == null)
            return IdentityErrors.ExternalLoginError("");

        var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
            return IdentityErrors.ExternalLoginError(nameof(email));

        var user = await _workUnit.UsersRepository.GetByEmailAsync(email);

        if (user == null)
            return UserErrors.NotFound(nameof(email));

        await _workUnit.IdentityRepository.SignInUserAsync(user);
        return Result.Success();
    }
}
