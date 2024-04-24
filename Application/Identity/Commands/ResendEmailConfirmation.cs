using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace CommunicationsApp.Application.Identity.Commands;

public sealed record ResendEmailConfirmationCommand : IRequest<bool>
{
    public string Email { get; set; }
}

public sealed class ResendEmailConfirmationCommandValidator : AbstractValidator<ResendEmailConfirmationCommand>
{
    public ResendEmailConfirmationCommandValidator()
    {
        RuleFor(e => e.Email).NotEmpty().EmailAddress();
    }
}

public sealed class ResendEmailConfirmationCommandHandler : BaseCommandHandler, IRequestHandler<ResendEmailConfirmationCommand, bool>
{
    public ResendEmailConfirmationCommandHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<bool> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _workUnit.UsersRepository.GetByEmailAsync(request.Email);

        if (user == null)
            return false;

        // trigger email to send
        // with events

        return true;
    }
}
