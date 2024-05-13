using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using MediatR;
using System.Text;

namespace CommunicationsApp.Application.Operations.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommmandHandler : BaseCommandHandler, IRequestHandler<ConfirmEmailCommmand, bool>
{
    public ConfirmEmailCommmandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<bool> Handle(ConfirmEmailCommmand request, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository
                                  .GetByIdAsync(request.UserId, excludeNonConfirmedEmail: false);

        if (user != null)
        {
            var decodedToken = Convert.FromBase64String(request.EmailConfirmationToken);

            return await _workUnit.UsersRepository
                                  .ConfirmEmailTokenAsync(user, Encoding.UTF8.GetString(decodedToken));
        }

        return false;
    }
}