using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Commands.DeleteMessage;

public record DeleteMessageCommand(int MessageId, int RequesterId) : IRequest<Result>;

public sealed class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(e => e.MessageId).NotEmpty();
        RuleFor(e => e.RequesterId).NotEmpty();
    }
}