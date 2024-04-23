using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Events;
using CommunicationsApp.Infrastructure;
using MediatR;

namespace CommunicationsApp.Application.Users.Commands;

public sealed class UserAddedEventHandler : INotificationHandler<UserAdded>
{
    private readonly IWorkUnit _workUnit;
    private readonly IEmailService _emailService;

    public UserAddedEventHandler(IWorkUnit workUnit, IEmailService emailService)
    {
        _workUnit = workUnit;
        _emailService = emailService;
    }

    public async Task Handle(UserAdded notification, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(notification.Id);

        if (user == null)
            return;

        string token = await _workUnit.UsersRepository.GetEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailConfirmationEmailAsync(null, null, token, notification.Email);
    }
}
