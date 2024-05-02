namespace CommunicationsApp.Application.DTOs;

public record Message(
    int Id, 
    string Text,
    DateTime CreatedAt,
    User User,
    DateTime? DeletedAt = null);