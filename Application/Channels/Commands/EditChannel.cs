using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Exceptions;
using CommunicationsApp.Infrastructure;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Channels.Commands;

public class EditChannelCommand : IRequest
{
    public int ChannelId { get; set; }
    public string NewName { get; set; }
}

public sealed class EditChannelCommandValidator : AbstractValidator<EditChannelCommand>
{
    public EditChannelCommandValidator()
    {
        RuleFor(e => e.ChannelId).NotEmpty();

        RuleFor(e => e.NewName)
            .NotEmpty()
            .MaximumLength(55);
    }
}

public sealed class EditChannelCommandHandler : BaseCommandHandler, IRequestHandler<EditChannelCommand>
{
    public EditChannelCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task Handle(EditChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await _workUnit.ChannelsRepository
                                     .GetByIdAsync(request.ChannelId);

        if (channel == null)
            throw new EntityNotFoundException("Channel");

        channel.Name = request.NewName;
        await _workUnit.SaveChangesAsync();
    }
}
