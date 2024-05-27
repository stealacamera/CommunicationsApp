using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Messages.Commands.DeleteMessage;

internal sealed class DeleteMessageCommandHandler : BaseCommandHandler, IRequestHandler<DeleteMessageCommand, Result<Message>>
{
    public DeleteMessageCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Message>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await IsRequestValidAsync(request, cancellationToken);

        if (validationResult.Failed)
            return validationResult.Errors.ToArray();

        var message = (await _workUnit.MessagesRepository
                                     .GetByIdAsync(request.MessageId, cancellationToken))!;

        // Edit message
        message.Text = "Message was deleted";
        message.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        // Remove message media
        await _workUnit.MediaRepository
                       .RemoveAllForMessage(message.Id, cancellationToken);

        var sender = (await _workUnit.UsersRepository
                                    .GetByIdAsync(message.OwnerId))!;

        return new Message(
            message.Id,
            message.Text,
            message.CreatedAt,
            new User(sender.Id, sender.UserName, sender.Email),
            new List<Media>(),
            message.DeletedAt);
    }

    private async Task<Result> IsRequestValidAsync(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        DeleteMessageCommandValidator validator = new();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return validationResult;

        var message = await _workUnit.MessagesRepository
                                     .GetByIdAsync(request.MessageId, cancellationToken);

        if (message == null)
            return MessageErrors.NotFound(nameof(request.MessageId));
        else if (message.OwnerId != request.RequesterId)
            return UserErrors.Unauthorized(nameof(request.RequesterId));

        return Result.Success();
    }
}