using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using MediatR;

namespace CommunicationsApp.Application.Operations.Identity.Commands.LogoutUser;

internal sealed class LogoutUserCommandHandler : BaseCommandHandler, IRequestHandler<LogoutUserCommand>
{
    public LogoutUserCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await _workUnit.UsersRepository.LogOutUserAsync();
    }
}
