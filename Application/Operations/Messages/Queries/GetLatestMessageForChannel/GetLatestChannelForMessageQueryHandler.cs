using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Common.Enums;
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
        if (!await _workUnit.ChannelsRepository.DoesInstanceExistAsync(request.ChannelId, cancellationToken))
            return ChannelErrors.NotFound;

        var message = await _workUnit.MessagesRepository
                                     .GetLatestForChannelAsync(request.ChannelId, cancellationToken);

        if (message != null)
        {
            var messageSender = await _workUnit.UsersRepository.GetByIdAsync(message.OwnerId);
            IList<Media> mediaModels = null;

            if (messageSender.DeletedAt == null)
            {
                var messageMedia = await _workUnit.MediaRepository.GetAllForMessage(message.Id, cancellationToken);

                mediaModels = messageMedia.Select(e => new Media(
                                                e.Filename, 
                                                MediaType.FromValue(e.MediaTypeId)))
                                          .ToList();
            }

            return new Message(
                message.Id,
                message.Text,
                message.CreatedAt,
                new User(messageSender.Id, messageSender.UserName, messageSender.Email),
                mediaModels,
                message.DeletedAt);
        }

        return null;
    }
}
