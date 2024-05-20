using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Common.Enums;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Queries.GetAllChannelsForUser;

public sealed class GetAllChannelsForUserQueryHandler
    : BaseCommandHandler, IRequestHandler<GetAllChannelsForUserQuery, Result<IList<Channel_BriefOverview>>>
{
    public GetAllChannelsForUserQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<IList<Channel_BriefOverview>>> Handle(GetAllChannelsForUserQuery request, CancellationToken cancellationToken)
    {
        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.UserId))
            return UserErrors.NotFound;

        return (await _workUnit.ChannelsRepository
                               .GetAllForUserAsync(request.UserId, cancellationToken))
                               .Select(e => CreateModel(e, cancellationToken))
                               .Select(e => e.Result)
                               .ToList();
    }

    private async Task<Channel_BriefOverview> CreateModel(Domain.Entities.Channel entity, CancellationToken cancellationToken)
    {
        var latestMessage = await _workUnit.MessagesRepository.GetLatestForChannelAsync(entity.Id, cancellationToken);
        Message messageModel = null;

        if (latestMessage != null)
        {
            var messageUser = await _workUnit.UsersRepository.GetByIdAsync(latestMessage.OwnerId);
            var messageUserModel = new User(messageUser.Id, messageUser.UserName, messageUser.Email);

            IList<Media> mediaModels = null;

            if (latestMessage.DeletedAt == null)
            {
                var messageMedia = await _workUnit.MediaRepository.GetAllForMessage(latestMessage.Id, cancellationToken);
                mediaModels = messageMedia.Select(e => new Media(
                                                     e.Filename,
                                                     MediaType.FromValue(e.MediaTypeId)))
                                          .ToList();
            }

            messageModel = new Message(
                latestMessage.Id,
                latestMessage.Text,
                latestMessage.CreatedAt,
                messageUserModel,
                mediaModels,
                latestMessage.DeletedAt);
        }

        return new Channel_BriefOverview(
            new Channel_BriefDescription(entity.Id, entity.Name, entity.Code), 
            messageModel);
    }
}
