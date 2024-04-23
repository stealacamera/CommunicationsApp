using CommunicationsApp.Application.Common;
using CommunicationsApp.Infrastructure;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Identity.Commands;

public sealed record ConfirmEmailCommmand : IRequest<bool>
{
    public int UserId;
    public string EmailConfirmationToken;
}

public sealed class ConfirmEmailCommmandValidator : AbstractValidator<ConfirmEmailCommmand>
{
    public ConfirmEmailCommmandValidator()
    {
        RuleFor(e => e.UserId).NotEmpty();
        RuleFor(e => e.EmailConfirmationToken).NotEmpty();
    }
}

public sealed class ConfirmEmailCommmandHandler : BaseCommandHandler, IRequestHandler<ConfirmEmailCommmand, bool>
{
    public ConfirmEmailCommmandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<bool> Handle(ConfirmEmailCommmand request, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository
                                  .GetByIdAsync(request.UserId);

        return user == null
               ? false
               : await _workUnit.UsersRepository
                                .IsEmailConfirmationTokenValidAsync(user, request.EmailConfirmationToken);
    }
}