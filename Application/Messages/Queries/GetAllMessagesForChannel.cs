using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Exceptions;
using CommunicationsApp.Infrastructure;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Messages.Queries;

public record GetAllMessagesForChannelCommand : IRequest
{
    public int ChannelId { get; set; }
    public int RequesterId { get; set; }
}

public sealed class GetAllMessagesForChannelCommandValidator : AbstractValidator<GetAllMessagesForChannelCommand>
{
    public GetAllMessagesForChannelCommandValidator()
    {
        RuleFor(e => e.ChannelId).NotEmpty();
        RuleFor(e => e.RequesterId).NotEmpty();
    }
}

public sealed class GetAllMessagesForChannelCommandHandler : BaseCommandHandler, IRequestHandler<GetAllMessagesForChannelCommand>
{
    public GetAllMessagesForChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task Handle(GetAllMessagesForChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId);

        //check if user is owner or member of channel

        if (channel == null)
            throw new EntityNotFoundException("Channel");
        else if (channel.OwnerId != 0)
            ;
    }
}