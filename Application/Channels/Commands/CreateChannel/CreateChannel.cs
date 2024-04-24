using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Exceptions;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Channels.Commands.CreateChannel;

public record CreateChannelCommand : IRequest<Channel>
{
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public IList<int> MemberIds { get; set; }
}

public sealed class CreateChannelCommandValidator : AbstractValidator<CreateChannelCommand>
{
    public CreateChannelCommandValidator()
    {
        RuleFor(e => e.OwnerId).NotEmpty();

        RuleFor(e => e.Name)
            .MaximumLength(55)
            .NotEmpty();

        RuleFor(e => e.MemberIds).NotEmpty();
    }
}

internal class CreateChannelCommandHandler : BaseCommandHandler, IRequestHandler<CreateChannelCommand, Channel>
{
    public CreateChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Channel> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        foreach (var userId in request.MemberIds)
            await CheckIfUserExistsAsync(userId);

        await CheckIfUserExistsAsync(request.OwnerId);

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

    private async Task CheckIfUserExistsAsync(int userId)
    {
        if (await _workUnit.UsersRepository.GetByIdAsync(userId) == null)
            throw new EntityNotFoundException("User");
    }
}

