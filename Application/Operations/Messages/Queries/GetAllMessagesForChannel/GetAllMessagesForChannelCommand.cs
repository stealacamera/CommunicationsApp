using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;

public record GetAllMessagesForChannelCommand(
    int ChannelId, 
    int RequesterId) 
    : IRequest<Result<IList<Message>>>;

public sealed class GetAllMessagesForChannelCommandValidator : AbstractValidator<GetAllMessagesForChannelCommand>
{
    public GetAllMessagesForChannelCommandValidator()
    {
        RuleFor(e => e.ChannelId).NotEmpty();
        RuleFor(e => e.RequesterId).NotEmpty();
    }
}