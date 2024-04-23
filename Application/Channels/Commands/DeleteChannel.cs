using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Exceptions;
using CommunicationsApp.Infrastructure;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Channels.Commands;

public record DeleteChannelCommand : IRequest
{
    public int ChannelId { get; set; }
}

public sealed class DeleteChannelCommandValidator : AbstractValidator<DeleteChannelCommand>
{
    public DeleteChannelCommandValidator()
    {
        RuleFor(e => e.ChannelId).NotEmpty();
    }
}

public sealed class DeleteChannelCommandHandler : BaseCommandHandler, IRequestHandler<DeleteChannelCommand>
{
    public DeleteChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository.GetByIdAsync(request.ChannelId);

        if (channel == null)
            throw new EntityNotFoundException("Channel");

        channel.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();
    }
}
