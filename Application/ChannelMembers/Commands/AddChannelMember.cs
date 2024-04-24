using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Exceptions;
using CommunicationsApp.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.ChannelMembers.Commands;

public record AddChannelMemberCommand : IRequest
{
    public int UserId { get; set; }
    public int ChannelId { get; set; }
}

public sealed class AddChannelMemberCommandValidator : AbstractValidator<AddChannelMemberCommand>
{
    public AddChannelMemberCommandValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.ChannelId).NotEmpty();
    }
}


internal class AddChannelMemberCommandHandler : BaseCommandHandler, IRequestHandler<AddChannelMemberCommand>
{
    public AddChannelMemberCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task Handle(AddChannelMemberCommand request, CancellationToken cancellationToken)
    {
        if (await _workUnit.UsersRepository.GetByIdAsync(request.UserId) == null)
            throw new EntityNotFoundException("User");

        if (await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId) == null)
            throw new EntityNotFoundException("Channel");

        var member = await _workUnit.ChannelMembersRepository
                                    .AddAsync(new Domain.Entities.ChannelMember
                                    {
                                        ChannelId = request.ChannelId,
                                        MemberId = request.UserId
                                    });

        await _workUnit.SaveChangesAsync();
        //return member;
    }
}
