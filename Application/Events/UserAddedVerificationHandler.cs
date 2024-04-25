using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Events;

public sealed class UserAddedVerificationHandler : BaseEmailVerificationHandler, INotificationHandler<UserAdded>
{
    public UserAddedVerificationHandler(
        IWorkUnit workUnit,
        IEmailService emailService)
        : base(workUnit, emailService)
    {
    }

    public async Task Handle(UserAdded notification, CancellationToken cancellationToken)
    {
        await SendConfirmationEmailByIdAsync(notification.Id, notification.EmailConfirmationBaseUrl);
    }
}