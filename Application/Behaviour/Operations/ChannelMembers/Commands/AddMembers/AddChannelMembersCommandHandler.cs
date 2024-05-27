using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Commands.AddMembers;

internal class AddChannelMembersCommandHandler : BaseCommandHandler, IRequestHandler<AddChannelMembersCommand, Result<IList<ChannelMember>>>
{
    private readonly IPublisher _publisher;

    public AddChannelMembersCommandHandler(
        IWorkUnit workUnit,
        IPublisher publisher) : base(workUnit)
    {
        _publisher = publisher;
    }

    public async Task<Result<IList<ChannelMember>>> Handle(AddChannelMembersCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        AddChannelMemberCommandValidator requestValidator = new();
        var validationResult = await requestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult;

        var channel = await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId, cancellationToken);

        if (channel == null)
            return ChannelErrors.NotFound(nameof(request.ChannelId));

        if (request.RequesterId.HasValue &&
            !await IsRequesterValid(request.RequesterId.Value, request.ChannelId, cancellationToken))
            return UserErrors.Unauthorized(nameof(request.RequesterId));
        
        // Add memberships
        var newMembers = new List<ChannelMember>();
        var channelModel = new Channel_BriefDescription(channel.Id, channel.Name, channel.Code);

        foreach (var userId in request.UserIds)
        {
            var createResult = await CreateMember(userId, request.Role, channelModel, cancellationToken);

            if (createResult.Succeded)
            {
                var member = createResult.Value;
                newMembers.Add(member);

                await _publisher.Publish(
                    new UserAddedToChannel(member.Member.Email, request.ChannelId, DateTime.Now),
                    cancellationToken);
            }
        }

        return newMembers;
    }

    private async Task<bool> IsRequesterValid(
        int requesterId, 
        int channelId, 
        CancellationToken cancellationToken)
    {
        // Check if user exists
        if (!await _workUnit.UsersRepository.DoesUserExistAsync(requesterId))
            return false;

        // Check if user is the owner of the given channel
        var requesterMembership = await _workUnit.ChannelMembersRepository
                                                 .GetByIdsAsync(requesterId, channelId, cancellationToken);

        if (requesterMembership == null || requesterMembership.RoleId != ChannelRole.Owner)
            return false;

        return true;
    }

    private async Task<Result<ChannelMember>> CreateMember(
        int userId,
        ChannelRole role,
        Channel_BriefDescription channelModel,
        CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByIdAsync(userId);

        // Validate if membership can be created
        if (user == null)
            return UserErrors.NotFound(nameof(AddChannelMembersCommand.UserIds));

        if (await _workUnit.ChannelMembersRepository
                           .IsUserMemberOfChannelAsync(userId, channelModel.Id, cancellationToken))
            return ChannelMemberErrors.AlreadyMemberOfChannel(nameof(AddChannelMembersCommand.UserIds));

        // Create membership
        await _workUnit.ChannelMembersRepository
                       .AddAsync(new Domain.Entities.ChannelMember
                       {
                           ChannelId = channelModel.Id,
                           MemberId = userId,
                           RoleId = role.Value
                       }, cancellationToken);

        await _workUnit.SaveChangesAsync();

        return new ChannelMember(
            new User(userId, user.UserName, user.Email),
            role,
            channelModel);
    }
}
