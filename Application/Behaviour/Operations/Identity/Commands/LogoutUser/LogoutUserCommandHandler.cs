using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.LogoutUser;

internal sealed class LogoutUserCommandHandler : BaseCommandHandler, IRequestHandler<LogoutUserCommand>
{
    public LogoutUserCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        LogoutUserCommandValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ArgumentException();

        if(!await _workUnit.UsersRepository.DoesUserExistAsync(request.RequesterId))
            throw new UnauthorizedAccessException();

        await _workUnit.IdentityRepository.LogOutUserAsync();
    }
}
