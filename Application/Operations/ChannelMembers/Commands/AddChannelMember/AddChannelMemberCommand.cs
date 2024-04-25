using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.ChannelMembers.Commands.AddChannelMember;

public record AddChannelMemberCommand(int UserId, int ChannelId) : IRequest<Result>;

public sealed class AddChannelMemberCommandValidator : AbstractValidator<AddChannelMemberCommand>
{
    public AddChannelMemberCommandValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.ChannelId).NotEmpty();
    }
}