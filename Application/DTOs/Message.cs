namespace CommunicationsApp.Application.DTOs;

public record Message(
    int Id, 
    string? Text,
    DateTime CreatedAt,
    User User,
    IList<Media>? Media,
    DateTime? DeletedAt = null);