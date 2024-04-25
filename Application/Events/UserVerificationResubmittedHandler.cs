using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Events;

public sealed class UserVerificationResubmittedHandler : BaseEmailVerificationHandler, INotificationHandler<UserVerificationResubmitted>
{
    public UserVerificationResubmittedHandler(
        IWorkUnit workUnit,
        IEmailService emailService)
        : base(workUnit, emailService)
    {
    }

    public async Task Handle(UserVerificationResubmitted notification, CancellationToken cancellationToken)
    {
        await SendConfirmationEmailByEmailAsync(notification.Email, notification.EmailVerificationBaseUrl);
    }
}
