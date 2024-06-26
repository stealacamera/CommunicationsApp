﻿using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Entities;
using System.Text;

namespace CommunicationsApp.Application.Behaviour.Events;

public abstract class BaseEmailVerificationHandler
{
    protected readonly IWorkUnit _workUnit;
    protected readonly IEmailService _emailService;

    public BaseEmailVerificationHandler(IWorkUnit workUnit, IEmailService emailService)
    {
        _workUnit = workUnit;
        _emailService = emailService;
    }

    protected async Task SendConfirmationEmailByEmailAsync(string email, string emailConfirmationBaseUrl)
    {
        var user = await _workUnit.UsersRepository
                                  .GetByEmailAsync(email, excludeNonConfirmedEmail: false);

        if (user == null)
            return;

        await SendConfirmationEmailAsync(user, emailConfirmationBaseUrl);
    }

    protected async Task SendConfirmationEmailByIdAsync(int id, string emailConfirmationBaseUrl)
    {
        var user = await _workUnit.UsersRepository
                                  .GetByIdAsync(id, excludeNonConfirmedEmail: false);

        if (user == null)
            return;

        await SendConfirmationEmailAsync(user, emailConfirmationBaseUrl);
    }

    private async Task SendConfirmationEmailAsync(User user, string emailConfirmationBaseUrl)
    {
        string token = await _workUnit.UsersRepository.GetEmailConfirmationTokenAsync(user);
        var tokenBytes = Encoding.UTF8.GetBytes(token);

        await _emailService.SendEmailConfirmationEmailAsync(
            emailConfirmationBaseUrl,
            user.Id,
            user.Email,
            Convert.ToBase64String(tokenBytes));
    }
}