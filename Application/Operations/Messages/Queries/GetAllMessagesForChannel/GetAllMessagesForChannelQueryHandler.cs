using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.DTOs.ViewModels;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Common.Enums;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;

public sealed class GetAllMessagesForChannelQueryHandler
    : BaseCommandHandler, IRequestHandler<GetAllMessagesForChannelQuery, Result<CursorPaginatedList<int, Message>>>
{
    public GetAllMessagesForChannelQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<CursorPaginatedList<int, Message>>> Handle(GetAllMessagesForChannelQuery request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId);

        if (channel == null)
            return ChannelErrors.NotFound;
        else if (!await _workUnit.ChannelMembersRepository.IsUserMemberOfChannelAsync(request.RequesterId, request.ChannelId))
            return ChannelMemberErrors.UserIsNotMemberOfChannel;

        var paginatedMessages = await _workUnit.MessagesRepository
                                               .GetAllForChannelAsync(request.ChannelId, request.Cursor, request.PageSize);

        var messages = paginatedMessages.Values
                                        .Select(ConvertEntityToModel)
                                        .Select(e => e.Result)
                                        .ToList();

        return new CursorPaginatedList<int, Message>(paginatedMessages.NextCursor, messages);
    }

    private async Task<Message> ConvertEntityToModel(Domain.Entities.Message entity)
    {
        // Get owner model
        var user = await _workUnit.UsersRepository.GetByIdAsync(entity.OwnerId);
        User messageOwner = new User(user.Id, user.UserName, user.Email);

        // Get media models
        var media = await _workUnit.MediaRepository.GetAllForMessage(entity.Id);
        IList<Media> mediaModels = media.Select(e => new Media(
                                                       e.Filename,
                                                       MediaType.FromValue(e.MediaTypeId)))
                                        .ToList();

        return new Message(
            entity.Id, entity.Text,
            entity.CreatedAt, messageOwner,
            mediaModels, entity.DeletedAt);
    }
}