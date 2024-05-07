using CommunicationsApp.Application.DTOs;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Queries.QueryByEmail;

public record QueryByEmailAndUsernameQuery(string Query, int? excludeRequesterId = null) : IRequest<IList<User>>;

public sealed class QueryByEmailAndUsernameQueryValidator : AbstractValidator<QueryByEmailAndUsernameQuery>
{
    public QueryByEmailAndUsernameQueryValidator()
    {
        RuleFor(e => e.Query)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(e => e.excludeRequesterId)
            .GreaterThan(0);
    }
}