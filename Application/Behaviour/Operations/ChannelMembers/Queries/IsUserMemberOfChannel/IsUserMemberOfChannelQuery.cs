using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.IsUserMemberOfChannel;

public record IsUserMemberOfChannelQuery(int UserId, int ChannelId) : IRequest<bool>;

internal sealed class IsUserMemberOfChannelQueryValidator : AbstractValidator<IsUserMemberOfChannelQuery>
{
    public IsUserMemberOfChannelQueryValidator()
    {
        RuleFor(e => e.UserId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);
    }
}
