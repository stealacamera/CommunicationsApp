using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.GetAllMembershipsForChannel;

public record GetAllMembershipsForChannelQuery(int ChannelId) : IRequest<Result<IList<ChannelMember>>>;

internal class GetAllMembershipsForChannelQueryValidator : AbstractValidator<GetAllMembershipsForChannelQuery>
{
    public GetAllMembershipsForChannelQueryValidator()
    {
        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);
    }
}