using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using MediatR;

namespace CommunicationsApp.Application.Operations.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommmandHandler : BaseCommandHandler, IRequestHandler<ConfirmEmailCommmand, bool>
{
    public ConfirmEmailCommmandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<bool> Handle(ConfirmEmailCommmand request, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository
                                  .GetByIdAsync(request.UserId);

        return user == null
               ? false
               : await _workUnit.UsersRepository
                                .IsEmailConfirmationTokenValidAsync(user, request.EmailConfirmationToken);
    }
}