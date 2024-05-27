using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Commands.CreateChannel;

internal sealed class CreateChannelCommandHandler
    : BaseCommandHandler, IRequestHandler<CreateChannelCommand, Result<Channel_BriefDescription>>
{
    private IPublisher _publisher;

    public CreateChannelCommandHandler(
        IWorkUnit workUnit,
        IPublisher publisher) : base(workUnit)
    {
        _publisher = publisher;
    }

    public async Task<Result<Channel_BriefDescription>> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        var requestValidation = await IsRequestValidAsync(request);

        if (requestValidation.Failed)
            return Result<Channel_BriefDescription>.Fail(requestValidation.Errors);

        return await WrapInTransactionAsync(async () =>
        {
            // Create channel
            var channel = await _workUnit.ChannelsRepository
                                     .AddAsync(new Domain.Entities.Channel
                                     {
                                         CreatedAt = DateTime.Now,
                                         Name = request.Name
                                     }, cancellationToken);

            await _workUnit.SaveChangesAsync();

            // Create memberships
            foreach (var memberId in request.MemberIds)
                await CreateMembership(memberId, channel.Id, cancellationToken);

            await _workUnit.ChannelMembersRepository
                           .AddAsync(new Domain.Entities.ChannelMember
                           {
                               ChannelId = channel.Id,
                               MemberId = request.OwnerId,
                               RoleId = ChannelRole.Owner.Value
                           }, cancellationToken);

            await _workUnit.SaveChangesAsync();

            return new Channel_BriefDescription(channel.Id, channel.Name, channel.Code);
        });
    }
    
    private async Task CreateMembership(int userId, int channelId, CancellationToken cancellationToken)
    {
        await _workUnit.ChannelMembersRepository
                               .AddAsync(new Domain.Entities.ChannelMember
                               {
                                   ChannelId = channelId,
                                   MemberId = userId,
                                   RoleId = ChannelRole.Member.Value
                               }, cancellationToken);

        var user = await _workUnit.UsersRepository.GetByIdAsync(userId);

        await _publisher.Publish(
            new UserAddedToChannel(user.Email, channelId, DateTime.Now),
            cancellationToken);
    }

    private async Task<Result> IsRequestValidAsync(CreateChannelCommand request)
    {
        // Validate request properies
        CreateChannelCommandValidator validator = new(maxNrMembers: 250);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return validationResult;

        // Validate nr members
        if (request.MemberIds.Contains(request.OwnerId))
            return ChannelMemberErrors.MemberIsOwner(nameof(request.MemberIds));

        // Validate users' existence
        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.OwnerId))
            return UserErrors.NotFound(nameof(request.MemberIds));
        
        foreach (var memberId in request.MemberIds)
        {
            if (!await _workUnit.UsersRepository.DoesUserExistAsync(memberId))
                return UserErrors.NotFound(nameof(request.MemberIds));
        }

        return Result.Success();
    }
}