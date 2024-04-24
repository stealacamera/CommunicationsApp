namespace CommunicationsApp.Domain.Abstractions;

public interface IEmailService
{
    Task SendEmailConfirmationEmailAsync(
        string baseUrl,
        int userId,
        string userEmail,
        string emailConfirmationToken);
}
