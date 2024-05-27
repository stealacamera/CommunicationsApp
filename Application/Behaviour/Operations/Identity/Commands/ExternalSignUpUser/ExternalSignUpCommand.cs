using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ExternalSignUpUser;

public record ExternalSignUpCommand() : IRequest<Result>;