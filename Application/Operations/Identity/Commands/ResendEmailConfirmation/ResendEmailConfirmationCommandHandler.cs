using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Operations.Identity.Commands.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationCommandHandler : BaseCommandHandler, IRequestHandler<ResendEmailConfirmationCommand, bool>
{
    private readonly IPublisher _publisher;

    public ResendEmailConfirmationCommandHandler(IWorkUnit workUnit, IPublisher publisher) : base(workUnit)
    {
        _publisher = publisher;
    }

    public async Task<bool> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByEmailAsync(request.Email, excludeNonConfirmedEmail: false);

        if (user == null)
            return false;

        await _publisher.Publish(
            new UserVerificationResubmitted(
                request.Email,
                request.EmailConfirmationUrl,
                DateTime.Now));

        return true;
    }
}