using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Queries.GetLatestMessageForChannel;

internal sealed class GetLatestChannelForMessageQueryHandler
    : BaseCommandHandler, IRequestHandler<GetLatestChannelForMessageQuery, Result<Message?>>
{
    public GetLatestChannelForMessageQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Message?>> Handle(GetLatestChannelForMessageQuery request, CancellationToken cancellationToken)
    {
        if (!await _workUnit.ChannelsRepository.DoesInstanceExistAsync(request.ChannelId))
            return ChannelErrors.NotFound;

        var message = await _workUnit.MessagesRepository.GetLatestForChannelAsync(request.ChannelId);

        if (message != null)
        {
            var messageSender = await _workUnit.UsersRepository.GetByIdAsync(message.OwnerId);

            return new Message(
                message.Id,
                message.Text,
                message.CreatedAt,
                new User(messageSender.Id, messageSender.UserName, messageSender.Email),
                message.DeletedAt);
        }

        return null;
    }
}
