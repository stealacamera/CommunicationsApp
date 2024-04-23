using MediatR;

namespace CommunicationsApp.Domain.Common;

public abstract record BaseEvent : INotification
{
    public DateTime OccurredAt { get; set; }
}
