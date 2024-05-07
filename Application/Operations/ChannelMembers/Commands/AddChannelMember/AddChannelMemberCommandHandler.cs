using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.ChannelMembers.Commands.AddChannelMember;

internal class AddChannelMemberCommandHandler : BaseCommandHandler, IRequestHandler<AddChannelMemberCommand, Result>
{
    public AddChannelMemberCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result> Handle(AddChannelMemberCommand request, CancellationToken cancellationToken)
    {
        if (await _workUnit.UsersRepository.GetByIdAsync(request.UserId) == null)
            return UserErrors.NotFound;

        if (await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId) == null)
            return ChannelErrors.NotFound;

        var member = await _workUnit.ChannelMembersRepository
                                    .AddAsync(new Domain.Entities.ChannelMember
                                    {
                                        ChannelId = request.ChannelId,
                                        MemberId = request.UserId
                                    });

        await _workUnit.SaveChangesAsync();
        return Result.Success();
    }
}
