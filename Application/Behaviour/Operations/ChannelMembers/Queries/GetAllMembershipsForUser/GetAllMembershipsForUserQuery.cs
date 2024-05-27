using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.GetAllMembershipsForUser;

public sealed record GetAllMembershipsForUserQuery(int UserId)
    : IRequest<Result<IList<ChannelMember>>>;

internal sealed class GetAllMembershipsForUserQueryValidator : AbstractValidator<GetAllMembershipsForUserQuery>
{
    public GetAllMembershipsForUserQueryValidator()
    {
        RuleFor(e => e.UserId)
            .NotEmpty()
            .GreaterThan(0);
    }
}