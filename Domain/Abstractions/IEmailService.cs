namespace CommunicationsApp.Domain.Abstractions;

public interface IEmailService
{
    Task SendEmailConfirmationEmailAsync(
        string baseUrl,
        int userId,
        string userEmail,
        string emailConfirmationToken);

    Task SendMemberRemovalEmailAsync(string userEmail, string channelName);
    Task SendMemberAddedEmailAsync(string userEmail, string channelName);
}
