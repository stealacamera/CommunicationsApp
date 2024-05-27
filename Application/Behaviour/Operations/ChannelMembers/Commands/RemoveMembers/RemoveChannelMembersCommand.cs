using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Commands.RemoveMembers;

public record RemoveChannelMembersCommand(
    int RequesterId,
    int ChannelId,
    params int[] MemberIds)
    : IRequest<Result>;

internal class RemoveChannelMembersCommandValidator : AbstractValidator<RemoveChannelMembersCommand>
{
    public RemoveChannelMembersCommandValidator()
    {
        RuleFor(e => e.RequesterId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.MemberIds)
            .NotEmpty()
            .ForEach(e => e.GreaterThan(0));
    }
}