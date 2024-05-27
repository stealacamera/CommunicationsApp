using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.DTOs.ViewModels;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Common.Enums;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Messages.Queries.GetAllMessagesForChannel;

internal sealed class GetAllMessagesForChannelQueryHandler
    : BaseCommandHandler, IRequestHandler<GetAllMessagesForChannelQuery, Result<CursorPaginatedList<int, Message>>>
{
    public GetAllMessagesForChannelQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<CursorPaginatedList<int, Message>>> Handle(GetAllMessagesForChannelQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await IsRequestValidAsync(request, cancellationToken);

        if (validationResult.Failed)
            return validationResult.Errors.ToArray();

        var paginatedMessages = await _workUnit.MessagesRepository
                                               .GetAllForChannelAsync(
                                                    request.ChannelId,
                                                    request.Cursor,
                                                    request.PageSize,
                                                    cancellationToken);

        var messages = paginatedMessages.Values
                                        .Select(e => ConvertEntityToModel(e, cancellationToken))
                                        .Select(e => e.Result)
                                        .ToList();

        return new CursorPaginatedList<int, Message>(paginatedMessages.NextCursor, messages);
    }

    private async Task<Result> IsRequestValidAsync(GetAllMessagesForChannelQuery request, CancellationToken cancellationToken)
    {
        GetAllMessagesForChannelQueryValidator validator = new();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult;

        var channel = await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken);

        if (channel == null)
            return ChannelErrors.NotFound(nameof(request.ChannelId));
        
        if (!await _workUnit.ChannelMembersRepository
                                 .IsUserMemberOfChannelAsync(request.RequesterId, request.ChannelId, cancellationToken))
            return ChannelMemberErrors.NotMemberOfChannel(nameof(request.RequesterId));

        return Result.Success();
    }

    private async Task<Message> ConvertEntityToModel(Domain.Entities.Message entity, CancellationToken cancellationToken)
    {
        // Get owner model
        var user = (await _workUnit.UsersRepository
                                   .GetByIdAsync(entity.OwnerId))!;
        
        User messageOwner = new User(user.Id, user.UserName, user.Email);

        // Get media models
        IList<Media>? mediaModels = null;

        if (entity.DeletedAt == null)
        {
            var media = await _workUnit.MediaRepository
                                       .GetAllForMessage(entity.Id, cancellationToken);

            mediaModels = media.Select(e => new Media(e.Filename, MediaType.FromValue(e.MediaTypeId)))
                               .ToList();
        }

        return new Message(
            entity.Id, entity.Text,
            entity.CreatedAt, messageOwner,
            mediaModels, entity.DeletedAt);
    }
}