using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ResendEmailConfirmation;

internal sealed class ResendEmailConfirmationCommandHandler : BaseCommandHandler, IRequestHandler<ResendEmailConfirmationCommand, bool>
{
    private readonly IPublisher _publisher;

    public ResendEmailConfirmationCommandHandler(IWorkUnit workUnit, IPublisher publisher) : base(workUnit)
    {
        _publisher = publisher;
    }

    public async Task<bool> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        ResendEmailConfirmationCommandValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return false;

        var user = await _workUnit.UsersRepository
                                  .GetByEmailAsync(request.Email, excludeNonConfirmedEmail: false);

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