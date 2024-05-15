using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CommunicationsApp.Application.Operations.Messages.Commands.CreateMessage;

public record CreateMessageCommand(
    string? Message, 
    int UserId, 
    int ChannelId,
    IFormFileCollection? Media) 
    : IRequest<Result<Message>>;

public sealed class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(e => e.Message).MaximumLength(1000);
        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.ChannelId).NotEmpty();

        //RuleFor(e => new { e.Message, e.Media})
        //    .Must(e => e.Message. !(e.Message != null && e.Media != null));
    }
}