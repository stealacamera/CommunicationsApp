using FluentValidation;

namespace CommunicationsApp.Web.Models;

public record Channel_AddRequestModel(string ChannelName, int[] MemberIds);

internal class ChannelValidator : AbstractValidator<Channel_AddRequestModel>
{
    public ChannelValidator()
    {
        RuleFor(e => e.ChannelName)
            .NotEmpty()
            .MaximumLength(55);

        RuleFor(e => e.MemberIds)
            .NotEmpty();
    }
}