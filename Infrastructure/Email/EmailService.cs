﻿using CommunicationsApp.Domain.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CommunicationsApp.Infrastructure.Email;

public class EmailService : IEmailService, IEmailSender
{
    private readonly EmailOptions _emailOptions;

    public EmailService(IOptions<EmailOptions> emailOptions) 
        => _emailOptions = emailOptions.Value;

    public async Task SendEmailConfirmationEmailAsync(
        string baseUrl,
        int userId,
        string userEmail, 
        string emailConfirmationToken)
    {
        var confirmationLink = baseUrl + $"?userId={userId}&token={emailConfirmationToken}";

        await SendEmailAsync(
            userEmail,
            "Account confirmation",
            "Please confirm your account by clicking <a href=\"" + confirmationLink + "\">here</a>");
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(MailboxAddress.Parse(_emailOptions.InfoDeskEmail));
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

        // TODO fix why email is not received
        // TODO outbox pattern for emails that dont send

        return Task.CompletedTask;
    }
}