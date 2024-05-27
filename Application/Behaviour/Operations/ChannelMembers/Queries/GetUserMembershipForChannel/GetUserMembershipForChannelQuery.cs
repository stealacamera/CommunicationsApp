using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.GetUserMembershipForChannel;

public record GetUserMembershipForChannelQuery(int UserId, int ChannelId) : IRequest<Result<ChannelMember?>>;

internal class GetUserMembershipForChannelQueryValidator : AbstractValidator<GetUserMembershipForChannelQuery>
{
    public GetUserMembershipForChannelQueryValidator()
    {
        RuleFor(e => e.UserId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);
    }
}

