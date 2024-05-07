using CommunicationsApp.Application.Common.Enums;

namespace CommunicationsApp.Application.DTOs;

public record ChannelMember(
    User Member,
    ChannelRole Role,
    Channel_BriefDescription Channel);
