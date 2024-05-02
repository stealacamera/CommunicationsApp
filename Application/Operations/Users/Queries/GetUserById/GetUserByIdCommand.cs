using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Queries.GetUserById;

public record GetUserByIdCommand(int Id) : IRequest<Result<User>>;

public sealed class GetUserByIdCommandValidator : AbstractValidator<GetUserByIdCommand>
{
    public GetUserByIdCommandValidator()
    {
        RuleFor(e => e.Id).NotEmpty();
    }
}