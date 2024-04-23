namespace CommunicationsApp.Domain.Abstractions;

public interface IEmailService
{
    Task SendEmailConfirmationEmailAsync(
        UrlHelper urlHelper,
        HttpRequest request,
        string emailConfirmationToken,
        string userId,
        string userEmail);
}
