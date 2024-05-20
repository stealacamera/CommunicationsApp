using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Events;

public record MemberRemovedFromChannel(
    string UserEmail,
    int ChannelId,
    DateTime OccuredAt)
    : BaseEvent(OccuredAt);