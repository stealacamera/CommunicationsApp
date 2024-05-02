using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;

public sealed class GetAllMessagesForChannelCommandHandler : BaseCommandHandler, IRequestHandler<GetAllMessagesForChannelCommand, Result<IList<Message>>>
{
    public GetAllMessagesForChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<IList<Message>>> Handle(GetAllMessagesForChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId);

        if (channel == null)
            return ChannelErrors.NotFound;
        else if (!await _workUnit.ChannelsRepository.DoesUserBelongToChannelAsync(request.RequesterId, request.ChannelId))
            return ChannelMemberErrors.UserIsNotMemberOfChannel;

        var messages = await _workUnit.MessagesRepository.GetAllForChannelAsync(request.ChannelId);

        return messages.Select(async e =>
                        {
                            var user = await _workUnit.UsersRepository.GetByIdAsync(e.OwnerId);
                            User messageOwner = new User(user.Id, user.UserName, user.Email);

                            return new Message(e.Id, e.Text, e.CreatedAt, messageOwner, e.DeletedAt);
                        })
                       .Select(e => e.Result)
                       .ToList();
    }
}