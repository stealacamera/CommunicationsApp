namespace CommunicationsApp.Application.DTOs;

public record Channel_BriefDescription(int Id, string Name);
public record Channel(
    int Id, 
    string Name, 
    DateTime CreatedAt, 
    string Code,
    User Owner,
    IList<User> Members);