using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Entities;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Commands.CreateChannel;

internal class CreateChannelCommandHandler : BaseCommandHandler, IRequestHandler<CreateChannelCommand, Result<Channel>>
{
    public CreateChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Channel>> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        if (request.MemberIds.Count == 1 && request.MemberIds[0] == request.OwnerId)
            return ChannelMemberErrors.MemberIsOwner;

        foreach (var memberId in request.MemberIds)
        {
            if (!await _workUnit.UsersRepository.DoesUserExistAsync(memberId))
                return UserErrors.NotFound;
        }

        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.OwnerId))
            return UserErrors.NotFound;

        return await WrapInTransactionAsync(async () =>
        {
            var channel = await _workUnit.ChannelsRepository
                                     .AddAsync(new Channel
                                     {
                                         OwnerId = request.OwnerId,
                                         CreatedAt = DateTime.Now,
                                         Name = request.Name
                                     });

            await _workUnit.SaveChangesAsync();

            foreach (var memberId in request.MemberIds)
                await _workUnit.ChannelMembersRepository
                               .AddAsync(new ChannelMember
                               {
                                   ChannelId = channel.Id,
                                   MemberId = memberId
                               });

            await _workUnit.SaveChangesAsync();
            return channel;
        });
    }
}