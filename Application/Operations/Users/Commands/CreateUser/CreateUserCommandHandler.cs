using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Entities;
using CommunicationsApp.Domain.Events;
using MediatR;

namespace CommunicationsApp.Application.Operations.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<User>>
{
    private readonly IWorkUnit _workUnit;
    private readonly IPublisher _publisher;

    public CreateUserCommandHandler(IWorkUnit workUnit, IPublisher publisher)
    {
        _workUnit = workUnit;
        _publisher = publisher;
    }

    public async Task<Result<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
        };

        var createUserResult = await _workUnit.UsersRepository.AddAsync(user, request.Password);

        if (!createUserResult.Succeeded)
            return new Error(createUserResult.Errors.First().Description);

        await _workUnit.SaveChangesAsync();

        await _publisher.Publish(
            new UserAdded(
                user.Id,
                user.Email,
                request.EmailConfirmationBaseUrl,
                DateTime.Now));

        return user;
    }
}
