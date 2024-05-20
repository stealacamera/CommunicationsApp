using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Events;

public record UserAddedToChannel(
    string UserEmail,
    int ChannelId,
    DateTime OccurredAt) 
    : BaseEvent(OccurredAt);