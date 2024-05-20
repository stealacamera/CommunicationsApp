using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Common.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CommunicationsApp.Application.Operations.Messages.Commands.CreateMessage;

public sealed class CreateMessageCommandHandler : BaseCommandHandler, IRequestHandler<CreateMessageCommand, Result<Message>>
{
    public CreateMessageCommandHandler(IWorkUnit workUnit) : base(workUnit) { }

    public async Task<Result<Message>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Message) && (request.Media == null || !request.Media.Any()))
            throw new ArgumentException();

        var user = await _workUnit.UsersRepository.GetByIdAsync(request.UserId);

        if (user == null)
            return UserErrors.NotFound;
        else if (!await _workUnit.ChannelsRepository.DoesInstanceExistAsync(request.ChannelId, cancellationToken))
            return ChannelErrors.NotFound;
        else if (!await _workUnit.ChannelMembersRepository
                                 .IsUserMemberOfChannelAsync(request.UserId, request.ChannelId, cancellationToken))
            return ChannelMemberErrors.NotFound;

        return await WrapInTransactionAsync(async () =>
        {
            // Create message
            var message = await _workUnit.MessagesRepository
                                         .AddAsync(new()
                                         {
                                             ChannelId = request.ChannelId,
                                             CreatedAt = DateTime.UtcNow,
                                             OwnerId = request.UserId,
                                             Text = string.IsNullOrEmpty(request.Message) ? null : request.Message,
                                         }, cancellationToken);

            await _workUnit.SaveChangesAsync();

            // Save files
            var media = await SaveFilesToDatabase(request.Media, message.Id, cancellationToken);
            await _workUnit.SaveChangesAsync();

            return new Message(
                message.Id,
                message.Text,
                message.CreatedAt,
                new(user.Id, user.UserName, user.Email),
                media,
                message.DeletedAt);
        });
    }

    private async Task<IList<Media>> SaveFilesToDatabase(IFormFileCollection files, int messageId, CancellationToken cancellationToken)
    {
        IList<Media> media = new List<Media>();

        if (files != null)
        {
            foreach (var file in files)
            {
                MediaType fileType;

                if (file.ContentType.Contains("image"))
                    fileType = MediaType.Image;
                else if (file.ContentType.Contains("video"))
                    fileType = MediaType.Video;
                else if (file.ContentType.Equals("application/pdf"))
                    fileType = MediaType.Document;
                else
                    throw new ArgumentException("Invalid media type");

                var mediaEntity = await _workUnit.MediaRepository
                                                 .AddAsync(new Domain.Entities.Media
                                                 {
                                                     Filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
                                                     MediaTypeId = fileType.Value,
                                                     MessageId = messageId
                                                 }, cancellationToken);

                media.Add(new Media(mediaEntity.Filename, fileType));
            }
        }

        return media;
    }
}
