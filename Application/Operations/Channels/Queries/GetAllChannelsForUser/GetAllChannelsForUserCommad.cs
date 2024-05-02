using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Channels.Queries.GetAllChannelsForUser;

public sealed record GetAllChannelsForUserCommad(int UserId) : IRequest<Result<IList<Channel_BriefDescription>>>;

public sealed class GetAllChannelsForUserCommadValidator : AbstractValidator<GetAllChannelsForUserCommad>
{
    public GetAllChannelsForUserCommadValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
    }
}
