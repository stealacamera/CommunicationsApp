using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Commands.CreateMessage;

public record CreateMessageCommand(
    string Message, 
    int UserId, 
    int ChannelId) 
    : IRequest<Result<Message>>;

public sealed class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(e => e.Message)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.ChannelId).NotEmpty();
    }
}