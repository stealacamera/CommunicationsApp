using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using MediatR;
using System.Text;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ConfirmEmail;

internal sealed class ConfirmEmailCommmandHandler : BaseCommandHandler, IRequestHandler<ConfirmEmailCommmand, bool>
{
    public ConfirmEmailCommmandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<bool> Handle(ConfirmEmailCommmand request, CancellationToken cancellationToken)
    {
        ConfirmEmailCommmandValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return false;

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