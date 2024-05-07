namespace CommunicationsApp.Application.DTOs;

public record Channel_BriefDescription(int Id, string Name, string Code);

public record Channel_BriefOverview(Channel_BriefDescription Channel, Message? LatestMessage);

public record Channel(
    int Id, 
    string Name, 
    DateTime CreatedAt, 
    string Code,
    IList<ChannelMember> Members);