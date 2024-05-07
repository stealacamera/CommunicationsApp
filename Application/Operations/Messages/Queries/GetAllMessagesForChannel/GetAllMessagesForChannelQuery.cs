using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;

public record GetAllMessagesForChannelQuery(
    int ChannelId, 
    int RequesterId) 
    : IRequest<Result<IList<Message>>>;

public sealed class GetAllMessagesForChannelQueryValidator : AbstractValidator<GetAllMessagesForChannelQuery>
{
    public GetAllMessagesForChannelQueryValidator()
    {
        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.RequesterId)
            .NotEmpty()
            .GreaterThan(0);
    }
}