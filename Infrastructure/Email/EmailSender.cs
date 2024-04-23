using CommunicationsApp.Domain.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CommunicationsApp.Infrastructure.Email;

public class EmailSender : IEmailService, IEmailSender
{
    private readonly EmailOptions _emailOptions;

    public EmailSender(IOptions<EmailOptions> emailOptions) 
        => _emailOptions = emailOptions.Value;

    public async Task SendEmailConfirmationEmailAsync(
        UrlHelper urlHelper, 
        HttpRequest request, 
        string emailConfirmationToken, 
        string userId, 
        string userEmail)
    {
        var confirmationLink = urlHelper.Action(
            "ConfirmEmail", 
            "Identity", 
            new { userId, emailConfirmationToken }, 
            request.Scheme);

        await SendEmailAsync(
            userEmail,
            "Account confirmation",
            "Please confirm your account by clicking <a href=\"" + confirmationLink + "\">here</a>");
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(MailboxAddress.Parse("info@artcommissions.com"));
        emailMessage.To.Add(MailboxAddress.Parse(email));

        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

        using (var emailClient = new SmtpClient())
        {
            emailClient.Connect(_emailOptions.Host, _emailOptions.Port, MailKit.Security.SecureSocketOptions.StartTls);
            emailClient.Authenticate(_emailOptions.InfoDeskEmail, _emailOptions.InfoDeskPassword);
            emailClient.Send(emailMessage);
            emailClient.Disconnect(true);
        }

        // TODO what to do if it doesnt send?

        return Task.CompletedTask;
    }
}
