using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.ChannelMembers.Commands.AddChannelMember;

internal class AddChannelMemberCommandHandler : BaseCommandHandler, IRequestHandler<AddChannelMemberCommand, Result<ChannelMember>>
{
    public AddChannelMemberCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<ChannelMember>> Handle(AddChannelMemberCommand request, CancellationToken cancellationToken)
    {
        if (await _workUnit.UsersRepository.GetByIdAsync(request.UserId) == null)
            return UserErrors.NotFound;
        else if (await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId) == null)
            return ChannelErrors.NotFound;

        if (await _workUnit.ChannelMembersRepository.IsUserMemberOfChannelAsync(request.UserId, request.ChannelId))
            return ChannelMemberErrors.UserAlreadyMemberOfChannel;

        await _workUnit.ChannelMembersRepository
                       .AddAsync(new Domain.Entities.ChannelMember
                       {
                           ChannelId = request.ChannelId,
                           MemberId = request.UserId,
                           RoleId = request.Role.Value
                       });

        await _workUnit.SaveChangesAsync();

        var senderUser = await _workUnit.UsersRepository.GetByIdAsync(request.UserId);
        var channel = await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId);

        return new ChannelMember(
            new User(senderUser.Id, senderUser.UserName, senderUser.Email),
            ChannelRole.Member,
            new Channel_BriefDescription(channel.Id, channel.Name, channel.Code));
    }
}
