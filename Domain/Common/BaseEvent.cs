using MediatR;

namespace CommunicationsApp.Domain.Common;

public abstract record BaseEvent(DateTime OccurredAt) : INotification;