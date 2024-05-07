using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Commands.CreateMessage;

public sealed class CreateMessageCommandHandler : BaseCommandHandler, IRequestHandler<CreateMessageCommand, Result<Message>>
{
    public CreateMessageCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Message>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(request.UserId);

        if (user == null)
            return UserErrors.NotFound;
        else if (!await _workUnit.ChannelsRepository.DoesInstanceExistAsync(request.ChannelId))
            return ChannelErrors.NotFound;
        else if (!await _workUnit.ChannelMembersRepository
                                 .IsUserMemberOfChannelAsync(request.UserId, request.ChannelId))
            return ChannelMemberErrors.NotFound;

        var message = await _workUnit.MessagesRepository
                                     .AddAsync(new()
                                     {
                                         ChannelId = request.ChannelId,
                                         CreatedAt = DateTime.UtcNow,
                                         OwnerId = request.UserId,
                                         Text = request.Message
                                     });

        await _workUnit.SaveChangesAsync();

        return new Message(
            message.Id,
            message.Text,
            message.CreatedAt,
            new(user.Id, user.UserName, user.Email),
            message.DeletedAt);
    }
}
