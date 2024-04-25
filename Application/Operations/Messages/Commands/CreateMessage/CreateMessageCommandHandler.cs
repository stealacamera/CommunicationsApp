using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Commands.CreateMessage;

public sealed class CreateMessageCommandHandler : BaseCommandHandler, IRequestHandler<CreateMessageCommand, Result>
{
    public CreateMessageCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.UserId))
            return UserErrors.NotFound;
        else if (!await _workUnit.ChannelsRepository.DoesInstanceExistAsync(request.ChannelId))
            return ChannelErrors.NotFound;
        else if (await _workUnit.ChannelMembersRepository.GetByIdsAsync(request.UserId, request.ChannelId) == null)
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
        return Result.Success();
        //return message;
    }
}
