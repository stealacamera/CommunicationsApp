using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.ChannelMembers.Queries.GetAllMembershipsForUser;

public sealed record GetAllMembershipsForUserCommand(int UserId)
    : IRequest<Result<IList<ChannelMember>>>;

internal sealed class GetAllMembershipsForUserCommandValidator : AbstractValidator<GetAllMembershipsForUserCommand>
{
    public GetAllMembershipsForUserCommandValidator()
    {
        RuleFor(e => e.UserId)
            .NotEmpty()
            .GreaterThan(0);
    }
}