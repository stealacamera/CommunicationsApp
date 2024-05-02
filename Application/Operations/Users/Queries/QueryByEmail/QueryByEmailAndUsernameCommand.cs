using CommunicationsApp.Application.DTOs;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Queries.QueryByEmail;

public record QueryByEmailAndUsernameCommand(string Query) : IRequest<IList<User>>;

public sealed class QueryByEmailAndUsernameCommandValidator : AbstractValidator<QueryByEmailAndUsernameCommand>
{
    public QueryByEmailAndUsernameCommandValidator()
    {
        RuleFor(e => e.Query).NotEmpty();
    }
}