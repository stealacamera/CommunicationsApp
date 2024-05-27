using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Common.Enums;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Messages.Queries.GetLatestMessageForChannel;

internal sealed class GetLatestChannelForMessageQueryHandler
    : BaseCommandHandler, IRequestHandler<GetLatestChannelForMessageQuery, Result<Message?>>
{
    public GetLatestChannelForMessageQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Message?>> Handle(GetLatestChannelForMessageQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await IsRequestValidAsync(request, cancellationToken);

        if (validationResult.Failed)
            return validationResult.Errors.ToArray();

        var message = await _workUnit.MessagesRepository
                                     .GetLatestForChannelAsync(request.ChannelId, cancellationToken);

        return message == null ?
               Result<Message?>.Success(null) :
               await GetMessageModelAsync(message, cancellationToken);
    }

    private async Task<Message> GetMessageModelAsync(Domain.Entities.Message entity, CancellationToken cancellationToken)
    {
        IList<Media>? mediaModels = null;
        var messageSender = (await _workUnit.UsersRepository
                                            .GetByIdAsync(entity.OwnerId))!;

        if (messageSender.DeletedAt == null)
        {
            var messageMedia = await _workUnit.MediaRepository
                                              .GetAllForMessage(entity.Id, cancellationToken);

            mediaModels = messageMedia.Select(e => new Media(
                                            e.Filename,
                                            MediaType.FromValue(e.MediaTypeId)))
                                      .ToList();
        }

        return new Message(
            entity.Id,
            entity.Text,
            entity.CreatedAt,
            new User(messageSender.Id, messageSender.UserName, messageSender.Email),
            mediaModels,
            entity.DeletedAt);
    }

    private async Task<Result> IsRequestValidAsync(GetLatestChannelForMessageQuery request, CancellationToken cancellationToken)
    {
        GetLatestChannelForMessageQueryValidation validator = new();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult;

        if (!await _workUnit.ChannelsRepository
                            .DoesInstanceExistAsync(request.ChannelId, cancellationToken))
            return ChannelErrors.NotFound(nameof(request.ChannelId));

        return Result.Success();
    }
}
