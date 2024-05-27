using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CommunicationsApp.Application.Behaviour.Operations.Messages.Commands.CreateMessage;

public record CreateMessageCommand(
    string? Message,
    int UserId,
    int ChannelId,
    IFormFileCollection? Media)
    : IRequest<Result<Message>>;

internal sealed class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(e => e.Message).MaximumLength(1000);
        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.ChannelId).NotEmpty();

        // At least a message or a media file should be present
        RuleFor(e => new { e.Media, e.Message })
            .Must(e => !(string.IsNullOrEmpty(e.Message) && (e.Media == null || !e.Media.Any())));
    }
}