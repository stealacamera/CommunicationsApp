using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Commands.CreateChannel;

internal sealed class CreateChannelCommandHandler 
    : BaseCommandHandler, IRequestHandler<CreateChannelCommand, Result<Channel_BriefDescription>>
{
    public CreateChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<Channel_BriefDescription>> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await IsRequestValid(request);

        if (validationResult.Failed)
            return validationResult.Error!;

        return await WrapInTransactionAsync(async () =>
        {
            var channel = await _workUnit.ChannelsRepository
                                     .AddAsync(new Domain.Entities.Channel
                                     {
                                         CreatedAt = DateTime.Now,
                                         Name = request.Name
                                     });

            await _workUnit.SaveChangesAsync();

            foreach (var memberId in request.MemberIds)
                await _workUnit.ChannelMembersRepository
                               .AddAsync(new Domain.Entities.ChannelMember
                               {
                                   ChannelId = channel.Id,
                                   MemberId = memberId,
                                   RoleId = ChannelRole.Member.Value
                               });

            await _workUnit.ChannelMembersRepository
                           .AddAsync(new Domain.Entities.ChannelMember
                           {
                               ChannelId = channel.Id,
                               MemberId = request.OwnerId,
                               RoleId = ChannelRole.Owner.Value
                           });

            await _workUnit.SaveChangesAsync();
            return new Channel_BriefDescription(channel.Id, channel.Name, channel.Code);
        });
    }

    private async Task<Result> IsRequestValid(CreateChannelCommand request)
    {
        int maxNrMembers = 250;

        // Validate nr members
        if (!request.MemberIds.Any() || request.MemberIds.Count > maxNrMembers)
            return ChannelErrors.IncorrectNumberOfMembers(maxNrMembers);
        else if (request.MemberIds.Contains(request.OwnerId))
            return ChannelMemberErrors.MemberIsOwner;

        // Validate users' existence
        foreach (var memberId in request.MemberIds)
        {
            if (!await _workUnit.UsersRepository.DoesUserExistAsync(memberId))
                return UserErrors.NotFound;
        }

        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.OwnerId))
            return UserErrors.NotFound;

        return Result.Success();
    }
}