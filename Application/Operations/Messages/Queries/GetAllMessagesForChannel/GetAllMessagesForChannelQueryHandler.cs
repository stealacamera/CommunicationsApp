using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.DTOs.ViewModels;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
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
                                        .Select(async e =>
                                         {
                                             var user = await _workUnit.UsersRepository.GetByIdAsync(e.OwnerId);
                                             User messageOwner = new User(user.Id, user.UserName, user.Email);

                                             return new Message(e.Id, e.Text, e.CreatedAt, messageOwner, e.DeletedAt);
                                         })
                                        .Select(e => e.Result)
                                        .ToList();

        return new CursorPaginatedList<int, Message>(paginatedMessages.NextCursor, messages);
}
}