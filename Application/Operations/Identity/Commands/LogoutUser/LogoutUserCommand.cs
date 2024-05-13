using MediatR;

namespace CommunicationsApp.Application.Operations.Identity.Commands.LogoutUser;

public record LogoutUserCommand() : IRequest;
