using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Users.Queries.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<Result<User>>;

internal sealed class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty()
            .GreaterThan(0);
    }
}