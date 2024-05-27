using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.DTOs.ViewModels;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Messages.Queries.GetAllMessagesForChannel;

public record GetAllMessagesForChannelQuery(
    int ChannelId,
    int RequesterId,
    int PageSize,
    int? Cursor = null)
    : IRequest<Result<CursorPaginatedList<int, Message>>>;

internal sealed class GetAllMessagesForChannelQueryValidator : AbstractValidator<GetAllMessagesForChannelQuery>
{
    public GetAllMessagesForChannelQueryValidator()
    {
        RuleFor(e => e.ChannelId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.RequesterId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(e => e.Cursor)
            .GreaterThanOrEqualTo(0);

        RuleFor(e => e.PageSize)
            .NotEmpty()
            .GreaterThan(0);
    }
}