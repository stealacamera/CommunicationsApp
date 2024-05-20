using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.ChannelMembers.Commands.AddMembers;

public record AddChannelMembersCommand(
    int? RequesterId, 
    int ChannelId, 
    ChannelRole Role, 
    params int[] UserIds) 
    : IRequest<Result<IList<ChannelMember>>>;

internal sealed class AddChannelMemberCommandValidator : AbstractValidator<AddChannelMembersCommand>
{
    public AddChannelMemberCommandValidator()
    {
        RuleFor(e => e.RequesterId)
            .GreaterThan(0);

        RuleFor(e => e.UserIds)
            .NotEmpty()
            .ForEach(e => e.GreaterThan(0));

        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.Role).NotEmpty();
    }
}