using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Events;

internal class UserAddedToChannelHandler : BaseCommandHandler, INotificationHandler<UserAddedToChannel>
{
    private readonly IEmailService _emailService;

    public UserAddedToChannelHandler(
        IWorkUnit workUnit,
        IEmailService emailService)
        : base(workUnit)
    {
        _emailService = emailService;
    }

    public async Task Handle(UserAddedToChannel notification, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByEmailAsync(notification.UserEmail);
        var channel = await _workUnit.ChannelsRepository.GetByIdAsync(notification.ChannelId, cancellationToken);

        if (user == null || channel == null)
            return;

        await _emailService.SendMemberAddedEmailAsync(user.Email, channel.Name);
    }
}
