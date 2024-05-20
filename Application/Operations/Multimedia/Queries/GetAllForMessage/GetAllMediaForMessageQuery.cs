using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Multimedia.Queries.GetAllForMessage;

public record GetAllMediaForMessageQuery(int MessageId) : IRequest<Result<IList<Media>>>;

internal sealed class GetAllMediaForMessageQueryValidator : AbstractValidator<GetAllMediaForMessageQuery>
{
    public GetAllMediaForMessageQueryValidator()
    {
        RuleFor(e => e.MessageId)
            .NotEmpty()
            .GreaterThan(0);
    }
}