using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.ChannelMembers.Commands.AddChannelMember;

public record AddChannelMemberCommand(int UserId, ChannelRole Role, int ChannelId) : IRequest<Result<ChannelMember>>;

public sealed class AddChannelMemberCommandValidator : AbstractValidator<AddChannelMemberCommand>
{
    public AddChannelMemberCommandValidator()
    {
        RuleFor(e => e.UserId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.Role).NotEmpty();
    }
}