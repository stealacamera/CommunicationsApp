using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Channels.Queries.GetAllChannelsForUser;

public sealed record GetAllChannelsForUserQuery(int UserId) : IRequest<Result<IList<Channel_BriefOverview>>>;

internal sealed class GetAllChannelsForUserQueryValidator : AbstractValidator<GetAllChannelsForUserQuery>
{
    public GetAllChannelsForUserQueryValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
    }
}
