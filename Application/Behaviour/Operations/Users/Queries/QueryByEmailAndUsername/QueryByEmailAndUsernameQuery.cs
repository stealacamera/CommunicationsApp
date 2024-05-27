using CommunicationsApp.Application.DTOs;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Users.Queries.QueryByEmailAndUsername;

public record QueryByEmailAndUsernameQuery(string Query, int? ExcludeRequesterId = null) : IRequest<IList<User>>;

internal sealed class QueryByEmailAndUsernameQueryValidator : AbstractValidator<QueryByEmailAndUsernameQuery>
{
    public QueryByEmailAndUsernameQueryValidator()
    {
        RuleFor(e => e.Query)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(e => e.ExcludeRequesterId)
            .GreaterThan(0);
    }
}