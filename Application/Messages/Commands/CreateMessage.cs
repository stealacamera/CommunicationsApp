using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Exceptions;
using CommunicationsApp.Infrastructure;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Messages.Commands;

public record CreateMessageCommand : IRequest
{
    public string Message { get; set; }
    public int UserId { get; set; }
    public int ChannelId { get; set; }
}

public sealed class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(e => e.Message)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.ChannelId).NotEmpty();
    }
}

public sealed class CreateMessageCommandHandler : BaseCommandHandler, IRequestHandler<CreateMessageCommand>
{
    public CreateMessageCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        if (!await _workUnit.UsersRepository.DoesUserExistAsync(request.UserId))
            throw new EntityNotFoundException("User");
        else if (!await _workUnit.ChannelsRepository.DoesInstanceExistAsync(request.ChannelId))
            throw new EntityNotFoundException("Channel");
        else if (await _workUnit.ChannelMembersRepository.GetByIdsAsync(request.UserId, request.ChannelId) == null)
            throw new EntityNotFoundException("Member");

        var message = await _workUnit.MessagesRepository
                                     .AddAsync(new()
                                     {
                                         ChannelId = request.ChannelId,
                                         CreatedAt = DateTime.UtcNow,
                                         OwnerId = request.UserId,
                                         Text = request.Message
                                     });

        await _workUnit.SaveChangesAsync();
        //return message;
    }
}