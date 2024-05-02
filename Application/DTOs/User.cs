namespace CommunicationsApp.Application.DTOs;

public record User(
    int Id, 
    string UserName, 
    string Email);