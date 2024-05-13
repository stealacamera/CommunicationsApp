using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Commands.DeleteMessage;

public sealed class DeleteMessageCommandHandler : BaseCommandHandler, IRequestHandler<DeleteMessageCommand, Result>
{
    public DeleteMessageCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _workUnit.MessagesRepository.GetByIdAsync(request.MessageId);

        if (message == null)
            return MessageErrors.NotFound;
        else if (message.OwnerId != request.RequesterId)
            return UserErrors.Unauthorized;

        message.Text = "Message was deleted";
        message.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return Result.Success();
    }
}