using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Events;

internal class MemberRemovedFromChannelHandler : BaseCommandHandler, INotificationHandler<MemberRemovedFromChannel>
{
    private readonly IEmailService _emailService;

    public MemberRemovedFromChannelHandler(
        IWorkUnit workUnit, 
        IEmailService emailService) 
        : base(workUnit)
    {
        _emailService = emailService;
    }

    public async Task Handle(MemberRemovedFromChannel notification, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByEmailAsync(notification.UserEmail);
        var channel = await _workUnit.ChannelsRepository.GetByIdAsync(notification.ChannelId, cancellationToken);

        if (user == null || channel == null)
            return;

        await _emailService.SendMemberRemovalEmailAsync(user.Email, channel.Name);
    }
}
