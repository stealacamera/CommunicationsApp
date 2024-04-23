using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Exceptions;
using CommunicationsApp.Infrastructure;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Messages.Commands;

public record DeleteMessageCommand : IRequest
{
    public int MessageId { get; set; }
    public int RequesterId { get; set; }
}

public sealed class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(e => e.MessageId).NotEmpty();
        RuleFor(e => e.RequesterId).NotEmpty();
    }
}

public sealed class DeleteMessageCommandHandler : BaseCommandHandler, IRequestHandler<DeleteMessageCommand>
{
    public DeleteMessageCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _workUnit.MessagesRepository.GetByIdAsync(request.MessageId);

        if (message == null)
            throw new EntityNotFoundException("Message");
        else if (message.OwnerId != request.RequesterId)
            throw new UnauthorizedException();

        message.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();
    }
}
