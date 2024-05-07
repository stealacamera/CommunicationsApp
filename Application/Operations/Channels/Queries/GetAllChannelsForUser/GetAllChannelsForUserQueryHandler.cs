using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;
using System.Runtime.InteropServices;

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
                               .GetAllForUserAsync(request.UserId))
                               .Select(async e => {
                                   var latestMessage = await _workUnit.MessagesRepository.GetLatestForChannelAsync(e.Id);
                                   Message messageModel = null;

                                   if(latestMessage != null)
                                   {
                                       var messageUser = await _workUnit.UsersRepository.GetByIdAsync(latestMessage.OwnerId);
                                       var messageUserModel = new User(messageUser.Id, messageUser.UserName, messageUser.Email);

                                       messageModel = new Message(
                                           latestMessage.Id, latestMessage.Text, 
                                           latestMessage.CreatedAt, messageUserModel, 
                                           latestMessage.DeletedAt);
                                   }

                                   return new Channel_BriefOverview(new Channel_BriefDescription(e.Id, e.Name, e.Code), messageModel);
                               })
                               .Select(e => e.Result)
                               .ToList();
    }
}
