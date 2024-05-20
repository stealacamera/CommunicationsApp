using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Commands.DeleteMessage;

public sealed class DeleteMessageCommandHandler : BaseCommandHandler, IRequestHandler<DeleteMessageCommand, Result<Message>>
{
    public DeleteMessageCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Message>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _workUnit.MessagesRepository
                                     .GetByIdAsync(request.MessageId, cancellationToken);

        if (message == null)
            return MessageErrors.NotFound;
        else if (message.OwnerId != request.RequesterId)
            return UserErrors.Unauthorized;

        // Edit message
        message.Text = "Message was deleted";
        message.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        // Remove message media
        await _workUnit.MediaRepository
                       .RemoveAllForMessage(message.Id, cancellationToken);

        var sender = await _workUnit.UsersRepository.GetByIdAsync(message.OwnerId);

        return new Message(
            message.Id, 
            message.Text, 
            message.CreatedAt,
            new User(sender.Id, sender.UserName, sender.Email),
            new List<Media>(),
            message.DeletedAt);
    }
}